using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class EnergySystem : MonoBehaviour
{
    [Header("Ajustes de Energía")]
    public float maxEnergy = 100f;
    public float currentEnergy = 100f;

    [Header("Tasas de Consumo")]
    public float idleEnergyDrainPerSecond = 1f;    // Consumo base estando quieto
    public float walkEnergyDrainPerSecond = 2f;    // Consumo adicional caminando
    public float runEnergyMultiplier = 2f;         // Multiplicador al correr
    
    // PUBLIC para ver en tiempo real el valor en el Inspector mientras juegas
    public float multiplicadorZona = 1f;           

    [Header("Colores de la Barra")]
    public Color colorNormal = Color.green;
    public Color colorCritico = Color.red;

    [Header("Ajustes de Tiempo")]
    public float tiempoTranscurrido = 0f;
    public bool juegoIniciado = false;
    private bool juegoTerminado = false;

    [Header("UI")]
    public Image energyFill;            
    public TMP_Text textoTiempo;        
    public TMP_Text gameOverText;       

    private PlayerController playerController;

    void Start()
    {
        Time.timeScale = 1f;
        currentEnergy = maxEnergy;
        playerController = GetComponent<PlayerController>();

        if (gameOverText != null) gameOverText.gameObject.SetActive(false);
        if (energyFill != null) energyFill.color = colorNormal;
    }

    void Update()
    {
        if (juegoTerminado) return;

        bool estaCaminando = false;
        bool estaCorriendo = false;

        // Detectar movimiento
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

        // Arranca el juego si te mueves O si entras en una zona de peligro
        if ((estaCaminando || multiplicadorZona > 1f) && !juegoIniciado)
        {
            juegoIniciado = true;
        }

        // CALCULOS Y CONSUMO DE ENERGÍA
        if (juegoIniciado)
        {
            tiempoTranscurrido += Time.deltaTime;
            ActualizarTextoTiempo();

            // Consumo base
            float consumoBase = idleEnergyDrainPerSecond;

            if (estaCaminando)
            {
                float factorCarrera = estaCorriendo ? runEnergyMultiplier : 1f;
                consumoBase += (walkEnergyDrainPerSecond * factorCarrera);
            }

            // APLICAR MULTIPLICADOR DE ZONA (Forzamos al menos 1f por seguridad)
            float factorZona = Mathf.Max(1f, multiplicadorZona);
            currentEnergy -= consumoBase * factorZona * Time.deltaTime;
            currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);

            if (currentEnergy <= 0f)
            {
                PerderJuego();
            }
        }

        // ACTUALIZAR BARRA DE ENERGÍA EN UI
        if (energyFill != null)
        {
            energyFill.fillAmount = currentEnergy / maxEnergy;
            energyFill.color = (currentEnergy <= (maxEnergy * 0.3f)) ? colorCritico : colorNormal;
        }
    }

    public void RecargarEnergia(float cantidad)
    {
        currentEnergy = Mathf.Clamp(currentEnergy + cantidad, 0f, maxEnergy);
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