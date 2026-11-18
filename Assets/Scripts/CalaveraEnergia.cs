
using UnityEngine;
using UnityEngine.UI;

public class CalaveraEnergia : MonoBehaviour
{
    [Header("Referencias")]
    public Slider barraEnergia;
    public RectTransform calavera;

    [Header("Ajuste visual")]
    public float desplazamientoX = 0f;

    void LateUpdate()
    {
        if (barraEnergia == null || calavera == null)
            return;

        Image imagenRelleno = barraEnergia.fillRect != null
            ? barraEnergia.fillRect.GetComponent<Image>()
            : null;

        if (imagenRelleno == null)
            return;

        RectTransform areaRelleno = imagenRelleno.rectTransform;
        RectTransform padreCalavera = calavera.parent as RectTransform;

        if (padreCalavera == null)
            return;

        // Porcentaje real del relleno verde
        float porcentaje = imagenRelleno.fillAmount;

        // Limites horizontales del rectangulo del relleno
        Rect rect = areaRelleno.rect;

        Vector3 puntoLocal = new Vector3(
            Mathf.Lerp(rect.xMin, rect.xMax, porcentaje),
            rect.center.y,
            0f
        );

        // Convertir la posicion local a coordenadas del mundo
        Vector3 puntoMundo =
            areaRelleno.TransformPoint(puntoLocal);

        // Convertir al espacio del padre de la calavera
        Vector3 puntoPadre =
            padreCalavera.InverseTransformPoint(puntoMundo);

        // Mover la calavera al extremo del relleno verde
        calavera.localPosition = new Vector3(
            puntoPadre.x + desplazamientoX,
            puntoPadre.y,
            calavera.localPosition.z
        );
    }
}

