using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Lo agrega SonidosUI a cada botón; no hace falta ponerlo a mano
public class SonidoBoton : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private Button boton;
    private Graphic grafico;
    private Color colorOriginal = Color.white;
    private Vector3 escalaOriginal = Vector3.one;
    private bool encima;
    private bool presionado;

    void Awake()
    {
        boton = GetComponent<Button>();
        escalaOriginal = transform.localScale;

        if (boton != null)
        {
            boton.onClick.AddListener(SonidosUI.ReproducirClick);
            boton.transition = Selectable.Transition.None; // el efecto lo hacemos nosotros
            grafico = boton.targetGraphic;
        }
        if (grafico == null) grafico = GetComponent<Graphic>();
        if (grafico != null) colorOriginal = grafico.color;
    }

    void OnEnable()
    {
        // Cada vez que el botón aparece, empieza en reposo
        encima = false;
        presionado = false;
        transform.localScale = escalaOriginal;
        if (grafico != null) grafico.color = ColorReposo();
    }

    void OnDestroy()
    {
        if (boton != null) boton.onClick.RemoveListener(SonidosUI.ReproducirClick);
    }

    Color ColorReposo()
    {
        float b = SonidosUI.BrilloReposo;
        return new Color(colorOriginal.r * b, colorOriginal.g * b, colorOriginal.b * b, colorOriginal.a);
    }

    bool PuedeUsarse()
    {
        return boton == null || boton.interactable;
    }

    void Update()
    {
        // Tiempo real: sigue animando aunque el juego esté en pausa (Time.timeScale = 0)
        float t = 1f - Mathf.Exp(-SonidosUI.VelocidadAnimacion * Time.unscaledDeltaTime);

        float factorEscala = presionado ? SonidosUI.EscalaPresionado
                           : (encima ? SonidosUI.EscalaHover : 1f);
        transform.localScale = Vector3.Lerp(transform.localScale, escalaOriginal * factorEscala, t);

        if (grafico != null)
        {
            Color destino = encima ? colorOriginal : ColorReposo();
            grafico.color = Color.Lerp(grafico.color, destino, t);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!PuedeUsarse()) return;
        encima = true;
        SonidosUI.ReproducirHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        encima = false;
        presionado = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (PuedeUsarse()) presionado = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        presionado = false;
    }
}