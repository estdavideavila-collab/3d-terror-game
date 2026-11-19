
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PanelNombre : MonoBehaviour
{
    [Header("Referencias")]
    public MenuController menuController;
    public GameObject panelNombre;
    public TMP_InputField campoNombre;
    public TMP_Text textoRanking;
    public TMP_Text textoAviso;

    [Header("Seleccion de dificultad")]
    public GameObject panelDificultad;

    private bool iniciandoPartida = false;

    void Start()
    {
        if (panelNombre != null)
            panelNombre.SetActive(false);

        if (panelDificultad != null)
            panelDificultad.SetActive(false);
    }

    void Update()
    {
        if (panelNombre != null &&
            panelNombre.activeInHierarchy &&
            !iniciandoPartida &&
            Keyboard.current != null)
        {
            if (Keyboard.current.enterKey.wasPressedThisFrame ||
                Keyboard.current.numpadEnterKey.wasPressedThisFrame)
            {
                Comenzar();
            }
        }
    }

    // NUEVO: lo llama JUGAR desde el menu principal
    public void AbrirDificultad()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        iniciandoPartida = false;

        // Facil predeterminado en cada partida nueva
        DificultadManager.dificultadActual =
            DificultadManager.ModoDificultad.Facil;

        if (menuController != null &&
            menuController.menuPrincipal != null)
        {
            menuController.menuPrincipal.SetActive(false);
        }

        if (panelNombre != null)
            panelNombre.SetActive(false);

        if (panelDificultad != null)
            panelDificultad.SetActive(true);
        else
            Debug.LogWarning("PanelNombre: falta PanelDificultad.");
    }

    // NUEVO: lo llama COMENZAR desde el panel de dificultad
    public void ContinuarDesdeDificultad()
    {
        if (panelDificultad != null)
            panelDificultad.SetActive(false);

        Abrir();
    }

    // Abrir el registro opcional de nombre
    public void Abrir()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        iniciandoPartida = false;

        if (menuController != null &&
            menuController.menuPrincipal != null)
        {
            menuController.menuPrincipal.SetActive(false);
        }

        if (panelDificultad != null)
            panelDificultad.SetActive(false);

        if (panelNombre != null)
            panelNombre.SetActive(true);

        if (campoNombre != null)
        {
            campoNombre.text = "";
            campoNombre.Select();
            campoNombre.ActivateInputField();
        }

        if (textoAviso != null)
        {
            textoAviso.text =
                "Escribe tu nombre para guardar tu record (opcional). " +
                "Presiona Enter para comenzar.";
        }

        ActualizarRanking();
    }

    // NUEVO: volver desde dificultad al menu principal
    public void VolverDesdeDificultad()
    {
        if (panelDificultad != null)
            panelDificultad.SetActive(false);

        if (menuController != null &&
            menuController.menuPrincipal != null)
        {
            menuController.menuPrincipal.SetActive(true);
        }
    }

    // Volver desde registro de nombre
    public void Volver()
    {
        if (panelNombre != null)
            panelNombre.SetActive(false);

        // Regresar al selector de dificultad
        if (panelDificultad != null)
            panelDificultad.SetActive(true);
        else if (menuController != null &&
                 menuController.menuPrincipal != null)
            menuController.menuPrincipal.SetActive(true);
    }

    // Comenzar partida desde el boton o Enter
    public void Comenzar()
    {
        if (iniciandoPartida)
            return;

        iniciandoPartida = true;

        string nombre = "";

        if (campoNombre != null)
            nombre = campoNombre.text.Trim();

        RankingManager.NombreJugador = nombre;

        if (panelNombre != null)
            panelNombre.SetActive(false);

        if (menuController != null)
        {
            menuController.Jugar();
        }
        else
        {
            Debug.LogWarning(
                "PanelNombre: falta asignar MenuController."
            );

            iniciandoPartida = false;
        }
    }

    void ActualizarRanking()
    {
        if (textoRanking == null)
            return;

        var lista = RankingManager.Cargar();

        if (lista.Count == 0)
        {
            textoRanking.text = "Aun no hay records";
            return;
        }

        var sb = new System.Text.StringBuilder();

        for (int i = 0; i < lista.Count; i++)
        {
            sb.AppendLine(
                (i + 1) + ". " +
                lista[i].nombre + " - " +
                RankingManager.Formato(lista[i].tiempo)
            );
        }

        textoRanking.text = sb.ToString();
    }
}
