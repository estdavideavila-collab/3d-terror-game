
using UnityEngine;

public class CansancioJugador : MonoBehaviour
{
    [Header("Configuracion del cansancio")]
    public float tiempoMaximoCorriendo = 5f;
    public float tiempoRecuperacion = 3f;

    [Header("Sonido de cansancio")]
    public AudioSource sonidoLatidos;

    private PlayerController jugador;
    private EnergySystem energia;

    private float tiempoCorriendo = 0f;
    private float tiempoRestante = 0f;
    private bool estaCansado = false;

    void Start()
    {
        jugador = GetComponent<PlayerController>();
        energia = GetComponent<EnergySystem>();

        // Aplicar dificultad predeterminada
        AplicarDificultad();

        if (jugador != null)
            jugador.PuedeCorrer = true;

        if (sonidoLatidos != null)
        {
            sonidoLatidos.playOnAwake = false;
            sonidoLatidos.loop = true;
            sonidoLatidos.Stop();
        }
    }

    // Aplica los tiempos del modo Facil o Dificil
    public void AplicarDificultad()
    {
        tiempoMaximoCorriendo =
            DificultadManager.TiempoMaximoCorriendo;

        tiempoRecuperacion =
            DificultadManager.TiempoRecuperacion;

        Debug.Log(
            "Cansancio configurado en modo: " +
            DificultadManager.dificultadActual
        );
    }

    void Update()
    {
        if (jugador == null || energia == null)
            return;

        // Detectar si la partida esta activa
        bool jugando =
            energia.juegoIniciado &&
            Time.timeScale > 0f &&
            Cursor.lockState == CursorLockMode.Locked;

        if (!jugando)
        {
            if (sonidoLatidos != null &&
                sonidoLatidos.isPlaying)
            {
                sonidoLatidos.Stop();
            }

            return;
        }

        // Recuperacion cuando el jugador esta agotado
        if (estaCansado)
        {
            tiempoRestante -= Time.deltaTime;

            if (tiempoRestante <= 0f)
            {
                estaCansado = false;
                tiempoCorriendo = 0f;
                tiempoRestante = 0f;

                jugador.PuedeCorrer = true;

                if (sonidoLatidos != null)
                    sonidoLatidos.Stop();
            }

            return;
        }

        // Acumular el tiempo mientras corre
        if (jugador.IsRunning)
        {
            tiempoCorriendo += Time.deltaTime;

            if (tiempoCorriendo >= tiempoMaximoCorriendo)
                AgotarJugador();
        }
    }

    void AgotarJugador()
    {
        estaCansado = true;
        tiempoRestante = tiempoRecuperacion;

        // Bloquear temporalmente la carrera
        jugador.PuedeCorrer = false;

        // Iniciar latidos durante el cansancio
        if (sonidoLatidos != null &&
            sonidoLatidos.clip != null)
        {
            sonidoLatidos.loop = true;
            sonidoLatidos.Play();
        }
    }

    void OnDisable()
    {
        if (sonidoLatidos != null)
            sonidoLatidos.Stop();

        if (jugador != null)
            jugador.PuedeCorrer = true;
    }
}
