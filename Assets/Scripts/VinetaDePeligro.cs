using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class VinetaDePeligro : MonoBehaviour
{
    [Header("Velocidad del efecto")]
    public float duracionDeAparicion = 0.4f;
    public float duracionDeDesaparicion = 0.6f;

    [Header("Intensidad maxima del rojo")]
    [Range(0f, 1f)]
    public float opacidadMaxima = 0.6f;

    private CanvasGroup grupoDeCanvas;
    private Coroutine corrutinaActual;

    void Awake()
    {
        grupoDeCanvas = GetComponent<CanvasGroup>();

        grupoDeCanvas.alpha = 0f;
        grupoDeCanvas.blocksRaycasts = false;
        grupoDeCanvas.interactable = false;
    }

    public void Mostrar()
    {
        if (corrutinaActual != null)
            StopCoroutine(corrutinaActual);

        corrutinaActual = StartCoroutine(DesvanecerHacia(opacidadMaxima, duracionDeAparicion));
    }

    public void Ocultar()
    {
        if (corrutinaActual != null)
            StopCoroutine(corrutinaActual);

        corrutinaActual = StartCoroutine(DesvanecerHacia(0f, duracionDeDesaparicion));
    }

    IEnumerator DesvanecerHacia(float opacidadObjetivo, float duracion)
    {
        float opacidadInicial = grupoDeCanvas.alpha;
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < duracion)
        {
            tiempoTranscurrido += Time.deltaTime;
            float progreso = tiempoTranscurrido / duracion;

            grupoDeCanvas.alpha = Mathf.Lerp(opacidadInicial, opacidadObjetivo, progreso);

            yield return null;
        }

        grupoDeCanvas.alpha = opacidadObjetivo;
    }
}