using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SonidoPasos : MonoBehaviour
{
    [Header("Clips de Pasos")]
    public AudioClip sonidoCaminar;
    public AudioClip sonidoCorrer;

    [Header("Ritmo de Pasos (segundos entre pisadas)")]
    public float intervaloCaminar = 0.45f;
    public float intervaloCorrer = 0.28f;

    [Header("Variación Procedural")]
    public float pitchMinimo = 0.88f;
    public float pitchMaximo = 1.12f;
    public float volumenMinimo = 0.5f;
    public float volumenMaximo = 0.7f;

    private PlayerController jugador;
    private AudioSource audioPasos;
    private float temporizadorPasos;

    void Start()
    {
        jugador = GetComponent<PlayerController>();
        audioPasos = GetComponent<AudioSource>();

        audioPasos.playOnAwake = false;
        audioPasos.loop = false; // Importante: desactivar loop continuo
        audioPasos.spatialBlend = 0f; // 2D para el jugador propio (o 1f para 3D posicional)
    }

    void Update()
    {
        if (jugador == null) return;

        bool jugando = Cursor.lockState == CursorLockMode.Locked;
        bool enSuelo = jugador.EstaEnSuelo;

        bool caminando = jugando && enSuelo && jugador.IsMoving && !jugador.IsRunning;
        bool corriendo = jugando && enSuelo && jugador.IsRunning;

        if (!caminando && !corriendo)
        {
            temporizadorPasos = 0f;
            return;
        }

        AudioClip clipActual = corriendo ? sonidoCorrer : sonidoCaminar;
        if (clipActual == null) return;

        float intervaloActual = corriendo ? intervaloCorrer : intervaloCaminar;

        temporizadorPasos += Time.deltaTime;
        if (temporizadorPasos >= intervaloActual)
        {
            temporizadorPasos = 0f;
            ReproducirPasoProcedural(clipActual);
        }
    }

    void ReproducirPasoProcedural(AudioClip clip)
    {
        // Cambia de tono y volumen en cada impacto de pisada
        audioPasos.pitch = Random.Range(pitchMinimo, pitchMaximo);
        float volRandom = Random.Range(volumenMinimo, volumenMaximo);
        audioPasos.PlayOneShot(clip, volRandom);
    }
}