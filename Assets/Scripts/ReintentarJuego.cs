
using UnityEngine;

public class ReintentarJuego : MonoBehaviour
{
    public void Reintentar()
    {
        Time.timeScale = 1f;

        MenuController menu = FindFirstObjectByType<MenuController>();

        if (menu != null)
        {
            menu.ReintentarJuego();
        }
        else
        {
            Debug.LogError("No se encontró MenuController.");
        }
    }
}
