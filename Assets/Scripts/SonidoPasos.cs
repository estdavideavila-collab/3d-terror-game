
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SonidoPasos : MonoBehaviour
{
    public AudioClip sonidoCaminar;
    public AudioClip sonidoCorrer;

    private PlayerController jugador;
    private AudioSource audioPasos;

    void Start()
    {
        jugador = GetComponent<PlayerController>();
        audioPasos = GetComponent<AudioSource>();

        audioPasos.playOnAwake = false;
        audioPasos.loop = true;
        audioPasos.spatialBlend = 0f;
        audioPasos.volume = 0.6f;
    }

    void Update()
    {
        if (jugador == null) return;

        // No reproducir pasos mientras esté abierto el menú.

        bool jugando = Cursor.lockState == CursorLockMode.Locked;
        bool enSuelo = jugador.EstaEnSuelo; // NUEVO: sin pasos en el aire

        bool caminando = jugando && enSuelo && jugador.IsMoving && !jugador.IsRunning;
        bool corriendo = jugando && enSuelo && jugador.IsRunning;

        if (!caminando && !corriendo)
        {
            if (audioPasos.isPlaying)
                audioPasos.Stop();

            return;
        }

        AudioClip sonidoActual = corriendo
            ? sonidoCorrer
            : sonidoCaminar;

        if (sonidoActual == null)
        {
            audioPasos.Stop();
            return;
        }

        if (audioPasos.clip != sonidoActual)
        {
            audioPasos.Stop();
            audioPasos.clip = sonidoActual;
        }

        if (!audioPasos.isPlaying)
        {
            audioPasos.Play();
        }
    }
}
