using UnityEngine;
using TMPro;

public class PanelNombre : MonoBehaviour
{
    [Header("Referencias")]
    public MenuController menuController;
    public GameObject panelNombre;
    public TMP_InputField campoNombre;
    public TMP_Text textoRanking;
    public TMP_Text textoAviso; // opcional

    void Start()
    {
        if (panelNombre != null) panelNombre.SetActive(false);
    }

    // Lo llama el botón JUGAR del menú principal
    public void Abrir()
    {
    // Restablecer el tiempo y liberar el cursor inmediatamente
    Time.timeScale = 1f;
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;

    if (menuController != null && menuController.menuPrincipal != null)
        menuController.menuPrincipal.SetActive(false);

    panelNombre.SetActive(true);
    campoNombre.text = RankingManager.NombreJugador; // Recordar el último nombre escrito
    if (textoAviso != null) textoAviso.text = "";

    // Forzar foco en la caja de texto para permitir teclear sin clics adicionales
    campoNombre.Select();
    campoNombre.ActivateInputField();

    ActualizarRanking();
    }

    // Botón VOLVER
    public void Volver()
    {
        panelNombre.SetActive(false);
        if (menuController != null && menuController.menuPrincipal != null)
            menuController.menuPrincipal.SetActive(true);
    }

    // Botón COMENZAR
    public void Comenzar()
    {
        string nombre = campoNombre.text.Trim();

        if (string.IsNullOrEmpty(nombre))
        {
            if (textoAviso != null) textoAviso.text = "Escribe tu nombre para continuar";
            return;
        }

        RankingManager.NombreJugador = nombre;
        panelNombre.SetActive(false);

        // Sigue el flujo de siempre: cinemática y luego el juego
        menuController.Jugar();
    }

    void ActualizarRanking()
    {
        var lista = RankingManager.Cargar();

        if (lista.Count == 0)
        {
            textoRanking.text = "Aún no hay récords";
            return;
        }

        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < lista.Count; i++)
        {
            sb.AppendLine((i + 1) + ". " + lista[i].nombre + "  -  " + RankingManager.Formato(lista[i].tiempo));
        }
        textoRanking.text = sb.ToString();
    }
}