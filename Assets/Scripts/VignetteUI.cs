using UnityEngine;
using UnityEngine.UI;

public class VignetteUI : MonoBehaviour
{
    public static VignetteUI Instance;
    private Image vignetteImage;
    
    [Header("Ajustes")]
    public float targetAlpha = 0.6f; // Opacidad máxima dentro de la zona
    public float fadeSpeed = 3f;

    private float currentAlpha = 0f;
    private bool isInDanger = false;

    void Awake()
    {
        Instance = this;
        vignetteImage = GetComponent<Image>();
    }

    void Update()
    {
        if (vignetteImage == null) return;

        float destination = isInDanger ? targetAlpha : 0f;
        currentAlpha = Mathf.MoveTowards(currentAlpha, destination, fadeSpeed * Time.deltaTime);

        Color c = vignetteImage.color;
        c.a = currentAlpha;
        vignetteImage.color = c;
    }

    public void SetDangerState(bool active)
    {
        isInDanger = active;
    }
}