using UnityEngine;
using UnityEngine.Video;
using UnityEngine.InputSystem;

public class CinematicaManager : MonoBehaviour
{
    [Header("Componentes de Video")]
    public VideoPlayer videoPlayer;
    public GameObject panelCinematica; // El objeto RawImage

    [Header("Referencias del Juego")]
    public MenuController menuController;
    public EnergySystem energySystem;
    public PlayerController playerController;

    private bool enCinematica = false;

    void Start()
    {
        if (panelCinematica != null)
            panelCinematica.SetActive(false);

        if (videoPlayer != null)
        {
            // Suscribirse al evento que se dispara cuando el video termina solo
            videoPlayer.loopPointReached += AlTerminarVideo;
        }
    }

    void Update()
    {
        // Detectar si el jugador presiona ENTER o INTRO para saltar
        if (enCinematica)
        {
            if (Keyboard.current != null && (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame))
            {
                SaltarCinematica();
            }
        }
    }

    public void ReproducirCinematica()
    {
        if (videoPlayer == null || panelCinematica == null)
        {
            FinalizarCinematicaEIniciarJuego();
            return;
        }

        enCinematica = true;
        panelCinematica.SetActive(true);
        videoPlayer.Play();
    }

    void AlTerminarVideo(VideoPlayer vp)
    {
        FinalizarCinematicaEIniciarJuego();
    }

    public void SaltarCinematica()
    {
        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
        }
        FinalizarCinematicaEIniciarJuego();
    }

    private void FinalizarCinematicaEIniciarJuego()
    {
        enCinematica = false;

        if (panelCinematica != null)
            panelCinematica.SetActive(false);

        // Activar Gameplay
        if (menuController != null && menuController.uiGameplay != null)
            menuController.uiGameplay.SetActive(true);

        if (energySystem != null)
            energySystem.IniciarGameplay();

        if (playerController != null)
            playerController.BloquearCursor();
    }
}