using UnityEngine;
using UnityEngine.UI;

public class SonidosUI : MonoBehaviour
{
    [Header("Sonidos de botones")]
    public AudioClip sonidoHover;   // al pasar el cursor por encima
    public AudioClip sonidoClick;   // al pulsar
    [Range(0f, 1f)] public float volumenHover = 0.4f;
    [Range(0f, 1f)] public float volumenClick = 0.8f;

    [Header("Efecto visual de los botones")]
    public float escalaHover = 1.1f;          // tamaño al pasar el cursor (1 = sin cambio)
    public float escalaPresionado = 0.96f;    // tamaño al pulsar
    [Range(0.3f, 1f)] public float brilloReposo = 0.8f; // 1 = sin atenuar; menor = más oscuro en reposo
    public float velocidadAnimacion = 14f;    // más alto = transición más rápida

    // Valores compartidos con SonidoBoton
    public static float EscalaHover = 1.1f;
    public static float EscalaPresionado = 0.96f;
    public static float BrilloReposo = 0.8f;
    public static float VelocidadAnimacion = 14f;

    // El AudioSource es estático y no se destruye al recargar la escena
    private static AudioSource fuente;
    private static AudioClip clipHover;
    private static AudioClip clipClick;
    private static float volHover;
    private static float volClick;

    void Awake()
    {
        clipHover = sonidoHover;
        clipClick = sonidoClick;
        volHover = volumenHover;
        volClick = volumenClick;

        EscalaHover = escalaHover;
        EscalaPresionado = escalaPresionado;
        BrilloReposo = brilloReposo;
        VelocidadAnimacion = velocidadAnimacion;

        if (fuente == null)
        {
            GameObject go = new GameObject("AudioUI");
            DontDestroyOnLoad(go);
            fuente = go.AddComponent<AudioSource>();
            fuente.playOnAwake = false;
            fuente.loop = false;
            fuente.spatialBlend = 0f;            // 2D
            fuente.ignoreListenerVolume = true;  // se oye aunque el audio del juego esté silenciado en el menú
        }
    }

    void Start()
    {
        // Le pone sonido y efecto a todos los botones de la escena, incluso a los de paneles ocultos
        Button[] botones = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Button b in botones)
        {
            if (b.GetComponent<SonidoBoton>() == null)
                b.gameObject.AddComponent<SonidoBoton>();
        }
    }

    public static void ReproducirHover()
    {
        if (fuente != null && clipHover != null) fuente.PlayOneShot(clipHover, volHover);
    }

    public static void ReproducirClick()
    {
        if (fuente != null && clipClick != null) fuente.PlayOneShot(clipClick, volClick);
    }
}