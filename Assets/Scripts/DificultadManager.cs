
using UnityEngine;

public class DificultadManager : MonoBehaviour
{
    public enum ModoDificultad
    {
        Facil,
        Dificil
    }

    // FACIL sigue siendo el modo predeterminado
    public static ModoDificultad dificultadActual =
        ModoDificultad.Facil;

    public static bool EsFacil =>
        dificultadActual == ModoDificultad.Facil;

    // Consumo de energia
    public static float ConsumoQuieto =>
        EsFacil ? 1f : 1.5f;

    public static float ConsumoCaminando =>
        EsFacil ? 2f : 3f;

    public static float MultiplicadorCarrera =>
        EsFacil ? 2f : 2.5f;

    // Penalizacion de zonas peligrosas
    public static float MultiplicadorZonaPeligrosa =>
        EsFacil ? 3f : 4.5f;

    // Sistema de cansancio
    public static float TiempoMaximoCorriendo =>
        EsFacil ? 3f : 2f;

    public static float TiempoRecuperacion =>
        EsFacil ? 5f : 7f;

    // Boton FACIL
    public void SeleccionarFacil()
    {
        dificultadActual = ModoDificultad.Facil;
        Debug.Log("Modo FACIL seleccionado");
    }

    // Boton DIFICIL
    public void SeleccionarDificil()
    {
        dificultadActual = ModoDificultad.Dificil;
        Debug.Log("Modo DIFICIL seleccionado");
    }
}

