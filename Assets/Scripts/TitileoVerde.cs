using UnityEngine;

public class TitileoVerde : MonoBehaviour
{
    [Header("Ajustes de Luz y Brillo")]
    public float velocidadTitileo = 5f;
    public float intensidadMinima = 0.5f;
    public float intensidadMaxima = 4f;

    private Light luzPunto;
    private Material materialInstancia;
    private Color colorVerde = new Color(0f, 1f, 0.2f); // Verde neón brillante

    void Start()
    {
        // 1. Configura la luz puntual para cortar la niebla
        luzPunto = GetComponent<Light>();
        if (luzPunto == null)
        {
            luzPunto = gameObject.AddComponent<Light>();
            luzPunto.type = LightType.Point;
            luzPunto.range = 7f;
            luzPunto.color = colorVerde;
        }

        // 2. Crea una instancia del material para modificar la emisión sin afectar el asset original
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            materialInstancia = renderer.material;
            materialInstancia.EnableKeyword("_EMISSION");
        }
    }

    void Update()
    {
        // Calcura el ciclo de titileo/pulsación
        float factor = (Mathf.Sin(Time.time * velocidadTitileo) + 1f) / 2f;
        float intensidadActual = Mathf.Lerp(intensidadMinima, intensidadMaxima, factor);

        // Hace parpadear la luz puntual
        if (luzPunto != null)
        {
            luzPunto.intensity = intensidadActual * 2.5f;
        }

        // Hace parpadear el brillo/emisión del material
        if (materialInstancia != null)
        {
            materialInstancia.SetColor("_EmissionColor", colorVerde * intensidadActual);
        }
    }
}