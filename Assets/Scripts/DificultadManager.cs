
using UnityEngine;

public class DificultadManager : MonoBehaviour
{
    public enum ModoDificultad
    {
        Facil,
        Dificil
    }

    // Facil por defecto
    public static ModoDificultad dificultadActual =
        ModoDificultad.Facil;

    public static bool EsFacil =>
        dificultadActual == ModoDificultad.Facil;

    // Consumo de energia
    public static float ConsumoQuieto =>
        EsFacil ? 0.3f : 1f;

    public static float ConsumoCaminando =>
        EsFacil ? 0.7f : 2f;

    public static float MultiplicadorCarrera =>
        EsFacil ? 1.5f : 2f;

    // Cansancio
    public static float TiempoMaximoCorriendo =>
        EsFacil ? 5f : 3f;

    public static float TiempoRecuperacion =>
        EsFacil ? 3f : 5f;

    // Seleccion desde botones del menu
    public void SeleccionarFacil()
    {
        dificultadActual = ModoDificultad.Facil;
        Debug.Log("Modo FACIL seleccionado");
    }

    public void SeleccionarDificil()
    {
        dificultadActual = ModoDificultad.Dificil;
        Debug.Log("Modo DIFICIL seleccionado");
    }
}
