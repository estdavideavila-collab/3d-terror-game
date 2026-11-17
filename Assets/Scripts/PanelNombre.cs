
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

    private bool iniciandoPartida = false;

    void Start()
    {
        if (panelNombre != null)
            panelNombre.SetActive(false);
    }

    void Update()
    {
        // Permitir continuar con Enter mientras el panel esté abierto
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

    // Lo llama el botón JUGAR del menú principal
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

        if (panelNombre != null)
            panelNombre.SetActive(true);

        // Campo vacío por defecto: registrar el nombre es opcional
        if (campoNombre != null)
        {
            campoNombre.text = "";
            campoNombre.Select();
            campoNombre.ActivateInputField();
        }

        if (textoAviso != null)
        {
            textoAviso.text =
                "Escribe tu nombre para guardar tu récord (opcional). " +
                "Presiona Enter para comenzar.";
        }

        ActualizarRanking();
    }

    // Botón VOLVER
    public void Volver()
    {
        if (panelNombre != null)
            panelNombre.SetActive(false);

        if (menuController != null &&
            menuController.menuPrincipal != null)
        {
            menuController.menuPrincipal.SetActive(true);
        }
    }

    // Botón COMENZAR o tecla ENTER
    public void Comenzar()
    {
        if (iniciandoPartida)
            return;

        iniciandoPartida = true;

        string nombre = "";

        if (campoNombre != null)
            nombre = campoNombre.text.Trim();

        // Vacío = partida sin registrar récord
        RankingManager.NombreJugador = nombre;

        if (panelNombre != null)
            panelNombre.SetActive(false);

        // Continuar con la cinemática y después el juego
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
            textoRanking.text = "Aún no hay récords";
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
