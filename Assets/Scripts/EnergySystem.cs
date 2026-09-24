using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class EnergySystem : MonoBehaviour
{
    [Header("Ajustes de Energía")]
    public float maxEnergy = 100f;
    public float currentEnergy = 100f;
    public float energyDrainPerSecond = 10f;       // Consumo base al caminar
    public float runEnergyMultiplier = 2.5f;       // Consumo 2.5x más rápido al correr
    public float multiplicadorDeConsumo = 1f;

    [Header("Colores de la Barra")]
    public Color colorNormal = Color.green;
    public Color colorCritico = Color.red;

    [Header("Ajustes de Tiempo")]
    public float tiempoTranscurrido = 0f;
    private bool juegoIniciado = false;
    private bool juegoTerminado = false;

    [Header("UI")]
    public Image energyFill;            // Objeto 'Fill' del Slider
    public TMP_Text textoTiempo;        // Texto del contador (00:00)
    public TMP_Text gameOverText;       // Texto de Game Over

    private PlayerController playerController;

    void Start()
    {
        Time.timeScale = 1f;
        currentEnergy = maxEnergy;
        playerController = GetComponent<PlayerController>();

        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
        }

        if (energyFill != null)
        {
            energyFill.color = colorNormal;
        }
    }

    void Update()
    {
        if (juegoTerminado) return;

        bool estaCaminando = false;
        bool estaCorriendo = false;

        // Comprobación directa con el PlayerController o lectura alternativa por teclado
        if (playerController != null)
        {
            estaCaminando = playerController.IsMoving;
            estaCorriendo = playerController.IsRunning;
        }
        else if (Keyboard.current != null)
        {
            estaCaminando = Keyboard.current.wKey.isPressed ||
                            Keyboard.current.aKey.isPressed ||
                            Keyboard.current.sKey.isPressed ||
                            Keyboard.current.dKey.isPressed;

            estaCorriendo = estaCaminando && 
                           (Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed);
        }

        // Arrancar cronómetro al primer paso
        if (estaCaminando && !juegoIniciado)
        {
            juegoIniciado = true;
        }

        // 1. CRONÓMETRO DE TIEMPO
        if (juegoIniciado)
        {
            tiempoTranscurrido += Time.deltaTime;
            ActualizarTextoTiempo();
        }

        // 2. CONSUMO DE ENERGÍA
        if (estaCaminando)
        {
            float factorCarrera = estaCorriendo ? runEnergyMultiplier : 1f;
            float consumoTotal = energyDrainPerSecond * factorCarrera * multiplicadorDeConsumo;

            currentEnergy -= consumoTotal * Time.deltaTime;
            currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);

            if (currentEnergy <= 0)
            {
                PerderJuego();
            }
        }

        // 3. BARRA Y CAMBIO DE COLOR
        if (energyFill != null)
        {
            energyFill.fillAmount = currentEnergy / maxEnergy;

            if (currentEnergy <= (maxEnergy * 0.3f))
            {
                energyFill.color = colorCritico;
            }
            else
            {
                energyFill.color = colorNormal;
            }
        }
    }

    void ActualizarTextoTiempo()
    {
        if (textoTiempo != null)
        {
            int minutos = Mathf.FloorToInt(tiempoTranscurrido / 60);
            int segundos = Mathf.FloorToInt(tiempoTranscurrido % 60);
            textoTiempo.text = string.Format("{0:00}:{1:00}", minutos, segundos);
        }
    }

    public void PerderJuego()
    {
        juegoTerminado = true;

        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.text = "¡HAS PERDIDO!";
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }
}