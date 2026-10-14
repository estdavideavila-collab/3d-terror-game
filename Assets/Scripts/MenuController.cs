using UnityEngine;

public class MenuController : MonoBehaviour
{
    [Header("Paneles de la UI")]
    public GameObject menuPrincipal;
    public GameObject explicacion;
    public GameObject uiGameplay;

    [Header("Referencia al Player")]
    public PlayerController playerController;

    private void Start()
    {
        // Aseguramos que el cursor empiece visible en el menú
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (menuPrincipal != null) menuPrincipal.SetActive(true);
        if (explicacion != null) explicacion.SetActive(false);
        if (uiGameplay != null) uiGameplay.SetActive(false);
    }

    public void Jugar()
    {
        Time.timeScale = 1f;

        // 1. Ocultar menús y mostrar la UI del gameplay
        if (menuPrincipal != null) menuPrincipal.SetActive(false);
        if (explicacion != null) explicacion.SetActive(false);
        if (uiGameplay != null) 
        {
            uiGameplay.SetActive(true);
            Canvas.ForceUpdateCanvases(); 
        }

        // 2. Iniciar el sistema de energía
        EnergySystem energy = FindFirstObjectByType<EnergySystem>();
        if (energy != null)
        {
            energy.IniciarGameplay();
        }

        // 3. Capturar el cursor para poder controlar la cámara
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

    public void AbrirComoJugar()
    {
        if (menuPrincipal != null) menuPrincipal.SetActive(false);
        if (explicacion != null) explicacion.SetActive(true);
    }

    public void CerrarComoJugar()
    {
        if (explicacion != null) explicacion.SetActive(false);
        if (menuPrincipal != null) menuPrincipal.SetActive(true);
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