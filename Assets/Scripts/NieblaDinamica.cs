using UnityEngine;

public class NieblaDinamica : MonoBehaviour
{
    [Header("Sistema de particulas de la niebla")]
    public ParticleSystem sistemaDeParticulasNiebla;

    [Header("Aumento inicial (pequena dinamica de entrada)")]
    public float tasaDeEmisionInicial = 0f;
    public float tasaDeEmisionConstante = 60f;
    public float duracionDelAumentoInicial = 4f;

    [Header("Niebla de fondo (opcional, RenderSettings)")]
    public bool usarNieblaDeFondo = true;
    public float densidadDeNieblaDeFondo = 0.05f;

    private float tiempoTranscurrido = 0f;
    private bool aumentoTerminado = false;

    void Start()
    {
        tiempoTranscurrido = 0f;
        aumentoTerminado = false;

        if (usarNieblaDeFondo)
        {
            // La niebla de fondo se deja fija desde el inicio,
            // solo la niebla de particulas tiene el aumento progresivo
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogDensity = densidadDeNieblaDeFondo;
        }

        if (sistemaDeParticulasNiebla != null)
        {
            var emision = sistemaDeParticulasNiebla.emission;
            emision.rateOverTime = tasaDeEmisionInicial;
        }
    }

    void Update()
    {
        // Una vez terminado el aumento inicial, la niebla se queda
        // constante durante el resto de la partida y no hace falta
        // seguir calculando nada en cada frame
        if (aumentoTerminado || sistemaDeParticulasNiebla == null)
            return;

        tiempoTranscurrido += Time.deltaTime;

        float progreso = Mathf.Clamp01(tiempoTranscurrido / duracionDelAumentoInicial);

        float tasaActual = Mathf.Lerp(
            tasaDeEmisionInicial,
            tasaDeEmisionConstante,
            progreso
        );

        var emision = sistemaDeParticulasNiebla.emission;
        emision.rateOverTime = tasaActual;

        if (progreso >= 1f)
        {
            aumentoTerminado = true;
        }
    }
}