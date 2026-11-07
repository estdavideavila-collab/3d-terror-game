using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RankingEntry
{
    public string nombre;
    public float tiempo;
}

[Serializable]
public class RankingData
{
    public List<RankingEntry> entradas = new List<RankingEntry>();
}

public static class RankingManager
{
    private const string KEY = "ranking_echo";
    private const int MAX_ENTRADAS = 10;

    // El nombre sobrevive cuando se recarga la escena al reintentar
    public static string NombreJugador = "";

    public static List<RankingEntry> Cargar()
    {
        string json = PlayerPrefs.GetString(KEY, "");
        if (string.IsNullOrEmpty(json)) return new List<RankingEntry>();

        RankingData data = JsonUtility.FromJson<RankingData>(json);
        return data != null ? data.entradas : new List<RankingEntry>();
    }

    public static void Agregar(string nombre, float tiempo)
    {
        List<RankingEntry> lista = Cargar();
        lista.Add(new RankingEntry { nombre = nombre, tiempo = tiempo });

        // Ordena de MAYOR a MENOR tiempo sobrevivido
        lista.Sort((a, b) => b.tiempo.CompareTo(a.tiempo));

        if (lista.Count > MAX_ENTRADAS)
            lista.RemoveRange(MAX_ENTRADAS, lista.Count - MAX_ENTRADAS);

        RankingData data = new RankingData { entradas = lista };
        PlayerPrefs.SetString(KEY, JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }

    public static string Formato(float segundosTotales)
    {
        int minutos = Mathf.FloorToInt(segundosTotales / 60f);
        int segundos = Mathf.FloorToInt(segundosTotales % 60f);
        return string.Format("{0:00}:{1:00}", minutos, segundos);
    }
}