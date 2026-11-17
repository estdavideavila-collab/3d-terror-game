
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("Paneles de la UI")]
    public GameObject menuPrincipal;
    public GameObject explicacion;
    public GameObject uiGameplay;

    [Header("Referencia al Player")]
    public PlayerController playerController;

    [Header("Cinemática")]
    public CinematicaManager cinematicaManager;

    // Permite saltar la cinemática únicamente al reintentar.
    private static bool reinicioSinCinematica = false;

    private void Start()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (explicacion != null)
            explicacion.SetActive(false);

        if (uiGameplay != null)
            uiGameplay.SetActive(false);

        if (reinicioSinCinematica)
        {
            reinicioSinCinematica = false;

            if (menuPrincipal != null)
                menuPrincipal.SetActive(false);

            // Esperar a que terminen de inicializarse los
            // demás componentes de la escena.
            StartCoroutine(IniciarTrasReinicio());
        }
        else
        {
            if (menuPrincipal != null)
                menuPrincipal.SetActive(true);
        }
    }

    private System.Collections.IEnumerator IniciarTrasReinicio()
    {
        yield return null;
        IniciarGameplayDirecto();
    }

    public void Jugar()
    {
        Time.timeScale = 1f;

        if (menuPrincipal != null)
            menuPrincipal.SetActive(false);

        if (explicacion != null)
            explicacion.SetActive(false);

        // La cinemática se reproduce en una partida nueva.
        if (cinematicaManager != null)
        {
            cinematicaManager.ReproducirCinematica();
        }
        else
        {
            IniciarGameplayDirecto();
        }
    }

    public void IniciarGameplayDirecto()
    {
        if (uiGameplay != null)
        {
            uiGameplay.SetActive(true);
            Canvas.ForceUpdateCanvases();
        }

        EnergySystem energy = FindFirstObjectByType<EnergySystem>();

        if (energy != null)
        {
            energy.IniciarGameplay();
        }

        if (playerController != null)
        {
            playerController.BloquearCursor();
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void ReintentarJuego()
    {
        // El siguiente inicio entra directamente al juego.
        reinicioSinCinematica = true;

        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }

    public void VolverAlMenuPrincipal()
        {
    // Restaurar el tiempo del juego
    Time.timeScale = 1f;

    // Indicar que el siguiente inicio debe mostrar el menú
    reinicioSinCinematica = false;

    // Liberar el cursor
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;

    // Recargar la escena desde el inicio
    SceneManager.LoadScene(
        SceneManager.GetActiveScene().name
    );
    }

    public void AbrirComoJugar()
    {
        if (menuPrincipal != null)
            menuPrincipal.SetActive(false);

        if (explicacion != null)
            explicacion.SetActive(true);
    }

    public void CerrarComoJugar()
    {
        if (explicacion != null)
            explicacion.SetActive(false);

        if (menuPrincipal != null)
            menuPrincipal.SetActive(true);
    }

    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
