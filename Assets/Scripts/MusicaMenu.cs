using UnityEngine;

[DefaultExecutionOrder(-100)] // se ejecuta antes que los demás para silenciar desde el primer momento
[RequireComponent(typeof(AudioSource))]
public class MusicaMenu : MonoBehaviour
{
    [Header("Música")]
    public AudioClip musica;
    [Range(0f, 1f)] public float volumen = 0.4f;
    public float fadeIn = 2f;      // segundos que tarda en aparecer
    public float fadeOut = 1.2f;   // segundos que tarda en desvanecerse

    [Header("Sonidos del juego mientras se está en el menú")]
    [Tooltip("Silencia ambiente, lobos y demás sonidos del juego mientras se ve el menú. La música y los sonidos de botones se siguen oyendo.")]
    public bool silenciarSonidosDelJuego = true;
    public float fadeSonidosJuego = 1.5f;   // segundos que tardan en volver al salir del menú

    [Header("La música suena mientras alguno de estos paneles esté activo")]
    public GameObject menuPrincipal;
    public GameObject panelNombre;
    public GameObject explicacion; // el panel de "Cómo jugar"

    private AudioSource fuente;
    private float nivel = 0f;          // fundido de la música: 0 = silencio, 1 = completo
    private float nivelJuego = 1f;     // fundido de los sonidos del juego: 0 = silenciados, 1 = normales
    private bool pausada = false;

    void Awake()
    {
        fuente = GetComponent<AudioSource>();
        fuente.clip = musica;
        fuente.loop = true;
        fuente.playOnAwake = false;
        fuente.spatialBlend = 0f; // 2D
        fuente.volume = 0f;
        fuente.ignoreListenerVolume = true; // la música se oye aunque se silencie todo lo demás

        if (silenciarSonidosDelJuego)
        {
            nivelJuego = 0f;
            AudioListener.volume = 0f;
        }
    }

    void OnDisable()
    {
        // Deja el volumen general normal al cerrar la escena o salir de Play
        AudioListener.volume = 1f;
    }

    void Update()
    {
        bool enMenu = EstaActivo(menuPrincipal) || EstaActivo(panelNombre) || EstaActivo(explicacion);
        float dt = Time.unscaledDeltaTime;

        // ---------- Sonidos del juego: silenciados en el menú, con fundido al salir ----------
        if (silenciarSonidosDelJuego)
        {
            float objetivoJuego = enMenu ? 0f : 1f;
            float duracionJuego = enMenu ? 0.3f : fadeSonidosJuego;
            nivelJuego = Mathf.MoveTowards(nivelJuego, objetivoJuego, dt / Mathf.Max(0.01f, duracionJuego));
            AudioListener.volume = nivelJuego * nivelJuego * (3f - 2f * nivelJuego);
        }

        // ---------- Música del menú ----------
        if (musica == null) return;

        if (enMenu)
        {
            if (pausada)
            {
                fuente.UnPause();
                pausada = false;
            }
            else if (!fuente.isPlaying)
            {
                fuente.Play();
            }
        }

        float objetivo = enMenu ? 1f : 0f;
        float duracion = enMenu ? fadeIn : fadeOut;
        nivel = Mathf.MoveTowards(nivel, objetivo, dt / Mathf.Max(0.01f, duracion));

        float curva = nivel * nivel * (3f - 2f * nivel); // smoothstep
        fuente.volume = volumen * curva;

        if (!enMenu && nivel <= 0f && fuente.isPlaying)
        {
            fuente.Pause();
            pausada = true;
        }
    }

    bool EstaActivo(GameObject go)
    {
        return go != null && go.activeInHierarchy;
    }
}