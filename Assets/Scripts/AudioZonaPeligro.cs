using UnityEngine;

public class AudioZonaPeligro : MonoBehaviour
{
    [Header("1) Guía de salida (ayuda a salir de la zona)")]
    public AudioClip sonidoGuia;
    [Range(0f, 1f)] public float volumenGuia = 0.8f;
    [Tooltip("Activado: suena sin parar (loop). Desactivado: suena en pulsos que se aceleran al acercarse a la salida.")]
    public bool guiaContinua = false;
    public float intervaloGuiaCerca = 0.35f;   // segundos entre pulsos cerca de la salida
    public float intervaloGuiaLejos = 1.2f;    // segundos entre pulsos lejos de la salida
    public float distanciaSalidaLejos = 10f;   // a esta distancia o más, ritmo lento
    public float tonoGuiaCerca = 1.25f;
    public float tonoGuiaLejos = 0.9f;
    public float margenSalida = 1.5f;          // metros fuera de la zona donde se coloca la guía
    public float distanciaMaximaGuia = 30f;    // hasta dónde se oye la guía
    public float fadeInGuia = 0.3f;            // segundos que tarda en aparecer al entrar
    public float fadeOutGuia = 1.5f;           // segundos que tarda en desvanecerse al salir

    [Header("2) Voces que distraen")]
    public AudioClip[] sonidosVoces;           // varios clips cortos, se elige uno al azar
    [Range(0f, 1f)] public float volumenVoces = 0.7f;
    public float intervaloVocesMin = 1.5f;     // segundos entre una voz y la siguiente
    public float intervaloVocesMax = 4f;
    public float distanciaVozMin = 1.5f;       // metros de tu cabeza
    public float distanciaVozMax = 5f;
    public float elevacionMin = -45f;          // grados: negativo = abajo
    public float elevacionMax = 60f;           // grados: positivo = arriba
    public float giroMaximo = 40f;             // grados por segundo que rota alrededor tuyo
    public float tonoVozMin = 0.85f;
    public float tonoVozMax = 1.1f;
    public int vocesSimultaneas = 3;
    public float fadeOutVoces = 1f;            // segundos que tardan en desvanecerse al salir

    [Header("3) Sonido de los items (botella y manzana) dentro de la zona")]
    [Range(0f, 1f)] public float factorItemsEnPeligro = 0.2f; // 0 = silencio, 1 = sin cambio
    public float tiempoBajarItems = 0.5f;      // segundos en bajar al entrar
    public float tiempoSubirItems = 1.5f;      // segundos en recuperar al salir

    [Header("Audio 3D")]
    [Tooltip("Si instalas un plugin de spatializer (ej. Steam Audio), activa HRTF real. Si no hay plugin, no hace nada.")]
    public bool usarSpatializer = true;

    private class Voz
    {
        public AudioSource fuente;
        public AudioLowPassFilter filtro;
        public float azimut, elevacion, distancia, giro;
    }

    private CharacterController controller;
    private Transform oyente;
    private AudioSource fuenteGuia;
    private Voz[] voces;
    private float temporizadorGuia;
    private float tiempoHastaVoz;
    private float fadeGuia = 0f;      // 0 = silencio, 1 = volumen completo
    private float factorVoces = 0f;   // 0 = silencio, 1 = volumen completo
    private readonly Collider[] buffer = new Collider[8];

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Por si la escena se recargó mientras el volumen de los items estaba bajo
        SonidoProximidad.FactorVolumenGlobal = 1f;

        // Emisor de la guía (en un hijo, no toca los AudioSource del Player)
        fuenteGuia = CrearFuente("GuiaSalida", 1f, distanciaMaximaGuia);
        fuenteGuia.volume = 0f;
        fuenteGuia.loop = guiaContinua;
        fuenteGuia.clip = guiaContinua ? sonidoGuia : null;

        // Emisores de las voces
        voces = new Voz[Mathf.Max(1, vocesSimultaneas)];
        for (int i = 0; i < voces.Length; i++)
        {
            Voz v = new Voz();
            v.fuente = CrearFuente("Voz_" + i, 1f, 15f);
            v.filtro = v.fuente.gameObject.AddComponent<AudioLowPassFilter>();
            v.filtro.cutoffFrequency = 22000f;
            voces[i] = v;
        }

        tiempoHastaVoz = Random.Range(0.8f, 2f);
    }

    void OnDestroy()
    {
        SonidoProximidad.FactorVolumenGlobal = 1f;
    }

    AudioSource CrearFuente(string nombre, float distMin, float distMax)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(transform, false);

        AudioSource a = go.AddComponent<AudioSource>();
        a.playOnAwake = false;
        a.loop = false;
        a.spatialBlend = 1f;                      // 3D
        a.rolloffMode = AudioRolloffMode.Linear;
        a.minDistance = distMin;
        a.maxDistance = distMax;
        a.spread = 0f;
        a.dopplerLevel = 0f;
        a.spatialize = usarSpatializer;
        return a;
    }

    void Update()
    {
        if (controller == null) return;

        bool juegoActivo = Cursor.lockState == CursorLockMode.Locked && Time.timeScale > 0f;

        if (oyente == null && Camera.main != null) oyente = Camera.main.transform;

        // Si el juego no está activo (menú, Game Over) se trata como "fuera de la zona"
        // y todo se desvanece suavemente en vez de cortarse
        Vector3 posBase = transform.position;
        bool enZona = false;
        if (juegoActivo && oyente != null) enZona = ZonaEn(posBase) != null;

        // Los fundidos usan tiempo real, así siguen funcionando con el juego en pausa
        ActualizarFades(enZona, Time.unscaledDeltaTime);
        ActualizarVoces();

        if (!enZona)
        {
            temporizadorGuia = 0f;
            tiempoHastaVoz = Random.Range(0.8f, 2f);
            return;
        }

        ActualizarGuia(posBase);
        ActualizarGeneradorDeVoces();
    }

    // ---------------- FUNDIDOS ----------------

    void ActualizarFades(bool enZona, float dt)
    {
        // Guía de salida
        float duracionGuia = enZona ? fadeInGuia : fadeOutGuia;
        fadeGuia = Mathf.MoveTowards(fadeGuia, enZona ? 1f : 0f, dt / Mathf.Max(0.01f, duracionGuia));
        fuenteGuia.volume = volumenGuia * fadeGuia;
        if (fadeGuia <= 0f && fuenteGuia.isPlaying) fuenteGuia.Stop();

        // Voces (aparecen rápido, se van en fadeOutVoces)
        float duracionVoces = enZona ? 0.2f : Mathf.Max(0.01f, fadeOutVoces);
        factorVoces = Mathf.MoveTowards(factorVoces, enZona ? 1f : 0f, dt / duracionVoces);

        // Items de energía: bajan al entrar, suben despacio al salir
        float objetivoItems = enZona ? factorItemsEnPeligro : 1f;
        float duracionItems = Mathf.Max(0.01f, enZona ? tiempoBajarItems : tiempoSubirItems);
        float paso = Mathf.Abs(1f - factorItemsEnPeligro) / duracionItems * dt;
        SonidoProximidad.FactorVolumenGlobal =
            Mathf.MoveTowards(SonidoProximidad.FactorVolumenGlobal, objetivoItems, paso);
    }

    // ---------------- DETECCIÓN DE ZONA ----------------

    // Devuelve la zona de peligro que toca la cápsula del jugador colocada en posBase
    Collider ZonaEn(Vector3 posBase)
    {
        Vector3 centro = posBase + controller.center;
        float mitad = Mathf.Max(0f, controller.height * 0.5f - controller.radius);
        Vector3 p1 = centro + Vector3.up * mitad;
        Vector3 p2 = centro - Vector3.up * mitad;

        int n = Physics.OverlapCapsuleNonAlloc(p1, p2, controller.radius, buffer, ~0, QueryTriggerInteraction.Collide);
        for (int i = 0; i < n; i++)
        {
            Collider c = buffer[i];
            if (c.GetComponentInParent<DangerousZone>() != null ||
                c.GetComponentInParent<ZonaPeligrosa>() != null)
                return c;
        }
        return null;
    }

    // Busca el punto de salida más cercano, fuera de todas las zonas que se superponen
    Vector3 CalcularSalida(Vector3 posBase)
    {
        Vector3 punto = posBase;
        for (int i = 0; i < 4; i++)
        {
            Collider zona = ZonaEn(punto);
            if (zona == null) break;
            punto = EmpujarFuera(punto, zona.bounds, margenSalida);
        }
        return punto;
    }

    // Mueve el punto hasta el borde más cercano de la zona (en el plano horizontal)
    Vector3 EmpujarFuera(Vector3 p, Bounds b, float margen)
    {
        float izq = p.x - b.min.x;
        float der = b.max.x - p.x;
        float atras = p.z - b.min.z;
        float frente = b.max.z - p.z;
        float menor = Mathf.Min(izq, der, atras, frente);

        if (menor == izq) p.x = b.min.x - margen;
        else if (menor == der) p.x = b.max.x + margen;
        else if (menor == atras) p.z = b.min.z - margen;
        else p.z = b.max.z + margen;
        return p;
    }

    // ---------------- GUÍA DE SALIDA ----------------

    void ActualizarGuia(Vector3 posBase)
    {
        if (sonidoGuia == null) return;

        Vector3 salida = CalcularSalida(posBase);
        salida.y = oyente.position.y; // a la altura de la cabeza
        fuenteGuia.transform.position = salida;

        Vector3 plano = salida - posBase;
        plano.y = 0f;
        float t = Mathf.Clamp01(plano.magnitude / Mathf.Max(0.1f, distanciaSalidaLejos)); // 0 = cerca, 1 = lejos

        fuenteGuia.pitch = Mathf.Lerp(tonoGuiaCerca, tonoGuiaLejos, t);

        if (guiaContinua)
        {
            if (!fuenteGuia.isPlaying) fuenteGuia.Play();
        }
        else
        {
            float intervalo = Mathf.Lerp(intervaloGuiaCerca, intervaloGuiaLejos, t);
            temporizadorGuia += Time.deltaTime;
            if (temporizadorGuia >= intervalo)
            {
                temporizadorGuia = 0f;
                fuenteGuia.PlayOneShot(sonidoGuia); // el volumen lo controla el fundido
            }
        }
    }

    // ---------------- VOCES QUE DISTRAEN ----------------

    void ActualizarGeneradorDeVoces()
    {
        if (sonidosVoces == null || sonidosVoces.Length == 0) return;

        tiempoHastaVoz -= Time.deltaTime;
        if (tiempoHastaVoz > 0f) return;

        tiempoHastaVoz = Random.Range(intervaloVocesMin, intervaloVocesMax);
        ReproducirVoz();
    }

    void ReproducirVoz()
    {
        Voz libre = null;
        foreach (Voz v in voces)
        {
            if (!v.fuente.isPlaying) { libre = v; break; }
        }
        if (libre == null) return;

        AudioClip clip = sonidosVoces[Random.Range(0, sonidosVoces.Length)];
        if (clip == null) return;

        libre.azimut = Random.Range(0f, 360f);
        libre.elevacion = Random.Range(elevacionMin, elevacionMax);
        libre.distancia = Random.Range(distanciaVozMin, distanciaVozMax);
        libre.giro = Random.Range(-giroMaximo, giroMaximo);
        PosicionarVoz(libre);

        // Simulación de altura: abajo = apagado, arriba = brillante
        float altura = Mathf.Clamp(libre.elevacion / 60f, -1f, 1f);
        libre.filtro.cutoffFrequency = Mathf.Lerp(1500f, 22000f, (altura + 1f) * 0.5f);

        libre.fuente.pitch = Random.Range(tonoVozMin, tonoVozMax) * (1f + 0.06f * altura);
        libre.fuente.clip = clip;
        libre.fuente.volume = volumenVoces * factorVoces;
        libre.fuente.Play();
    }

    // Mueve las voces que están sonando y aplica su fundido
    void ActualizarVoces()
    {
        if (oyente == null || voces == null) return;

        foreach (Voz v in voces)
        {
            if (!v.fuente.isPlaying) continue;

            v.azimut += v.giro * Time.deltaTime;
            PosicionarVoz(v);

            v.fuente.volume = volumenVoces * factorVoces;
            if (factorVoces <= 0f) v.fuente.Stop();
        }
    }

    void PosicionarVoz(Voz v)
    {
        float az = v.azimut * Mathf.Deg2Rad;
        float el = v.elevacion * Mathf.Deg2Rad;
        Vector3 dir = new Vector3(
            Mathf.Sin(az) * Mathf.Cos(el),
            Mathf.Sin(el),
            Mathf.Cos(az) * Mathf.Cos(el));
        v.fuente.transform.position = oyente.position + dir * v.distancia;
    }
}