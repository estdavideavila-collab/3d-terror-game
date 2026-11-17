
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SonidoPasos : MonoBehaviour
{
    [Header("Clips de Pasos")]
    public AudioClip sonidoCaminar;
    public AudioClip sonidoCorrer;

    [Header("Ritmo de Pasos")]
    public float intervaloCaminar = 0.45f;
    public float intervaloCorrer = 0.28f;

    [Header("Variacion Procedural")]
    public float pitchMinimo = 0.88f;
    public float pitchMaximo = 1.12f;
    public float volumenMinimo = 0.5f;
    public float volumenMaximo = 0.7f;

    private PlayerController jugador;
    private AudioSource audioPasos;
    private float temporizadorPasos = 0f;

    void Start()
    {
        jugador = GetComponent<PlayerController>();
        audioPasos = GetComponent<AudioSource>();

        audioPasos.playOnAwake = false;
        audioPasos.loop = true;
        audioPasos.spatialBlend = 0f;

        audioPasos.Stop();
    }

    void Update()
    {
        if (jugador == null || audioPasos == null)
            return;

        bool jugando =
            Cursor.lockState == CursorLockMode.Locked &&
            Time.timeScale > 0f;

        bool enSuelo = jugador.EstaEnSuelo;

        bool caminando =
            jugando &&
            enSuelo &&
            jugador.IsMoving &&
            !jugador.IsRunning;

        bool corriendo =
            jugando &&
            enSuelo &&
            jugador.IsRunning;

        // Detener inmediatamente las pisadas
        // cuando el personaje deja de moverse.
        if (!caminando && !corriendo)
        {
            DetenerPasos();
            return;
        }

        AudioClip clipActual =
            corriendo ? sonidoCorrer : sonidoCaminar;

        if (clipActual == null)
        {
            DetenerPasos();
            return;
        }

        // Cambiar inmediatamente entre caminar y correr.
        if (audioPasos.clip != clipActual ||
            !audioPasos.isPlaying)
        {
            audioPasos.Stop();
            audioPasos.clip = clipActual;

            AplicarVariacionProcedural();

            audioPasos.Play();
            temporizadorPasos = 0f;
        }

        // Variaciones suaves mientras se mueve
        float intervaloActual =
            corriendo ? intervaloCorrer : intervaloCaminar;

        temporizadorPasos += Time.deltaTime;

        if (temporizadorPasos >= intervaloActual)
        {
            temporizadorPasos = 0f;
            AplicarVariacionProcedural();
        }
    }

    void AplicarVariacionProcedural()
    {
        audioPasos.pitch =
            Random.Range(pitchMinimo, pitchMaximo);

        audioPasos.volume =
            Random.Range(volumenMinimo, volumenMaximo);
    }

    void DetenerPasos()
    {
        if (audioPasos.isPlaying)
            audioPasos.Stop();

        temporizadorPasos = 0f;
    }

    void OnDisable()
    {
        if (audioPasos != null)
            audioPasos.Stop();
    }
}
