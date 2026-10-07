using UnityEngine;

public class NieblaDinamica : MonoBehaviour
{
    [Header("Visibilidad Normal (Fuera de Peligro)")]
    public float visibilidadMinima = 5f;  // Lo más cerca que se pone la niebla (casi a ciegas)
    public float visibilidadMaxima = 12f; // Lo más lejos que se puede ver
    public float velocidadVariacion = 0.5f; // Qué tan rápido cambia la niebla sola

    [Header("Visibilidad en Zona de Peligro")]
    public float visibilidadEnPeligro = 3f; // Visibilidad extrema al entrar en zona roja
    public float velocidadTransicionPeligro = 3f;

    [Header("Color de la Niebla")]
    public Color colorBase = new Color(0.1f, 0.1f, 0.12f); // Gris muy oscuro/tenebroso
    public Color colorPeligro = new Color(0.3f, 0.05f, 0.05f); // Tono rojizo sutil en peligro

    private bool enZonaPeligro = false;
    private float distanciaActual;

    void Start()
    {
        // Activa la niebla por código
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = 0f; // Empieza pegada al personaje
        RenderSettings.fogColor = colorBase;

        distanciaActual = visibilidadMaxima;

        // Ajusta el fondo de la cámara para que coincida con el color de la niebla
        if (Camera.main != null)
        {
            Camera.main.clearFlags = CameraClearFlags.SolidColor;
            Camera.main.backgroundColor = colorBase;
        }
    }

    void Update()
    {
        if (enZonaPeligro)
        {
            // Transición rápida a niebla super densa y rojiza
            distanciaActual = Mathf.Lerp(distanciaActual, visibilidadEnPeligro, Time.deltaTime * velocidadTransicionPeligro);
            RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, colorPeligro, Time.deltaTime * velocidadTransicionPeligro);
        }
        else
        {
            // La niebla "respira" suavemente entre visibilidadMinima y visibilidadMaxima usando PerlinNoise
            float fluctuacion = Mathf.PerlinNoise(Time.time * velocidadVariacion, 0f);
            float distanciaObjetivo = Mathf.Lerp(visibilidadMinima, visibilidadMaxima, fluctuacion);

            distanciaActual = Mathf.Lerp(distanciaActual, distanciaObjetivo, Time.deltaTime * velocidadVariacion);
            RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, colorBase, Time.deltaTime * velocidadVariacion);
        }

        RenderSettings.fogEndDistance = distanciaActual;

        if (Camera.main != null)
        {
            Camera.main.backgroundColor = RenderSettings.fogColor;
        }
    }

    // Métodos para conectar con la zona de peligro
    public void EntrarEnPeligro() => enZonaPeligro = true;
    public void SalirDePeligro() => enZonaPeligro = false;
}