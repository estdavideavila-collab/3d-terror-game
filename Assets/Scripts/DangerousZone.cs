using UnityEngine;

public class DangerousZone : MonoBehaviour
{
    [Header("Consumo de Energía en Zona Peligrosa")]
    public float dañoEnergiaPorSegundo = 20f; // Cantidad extra de energía que quita por segundo

    private EnergySystem energySystem;
    private bool jugadorDentro = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            energySystem = other.GetComponent<EnergySystem>();
            if (energySystem == null) energySystem = FindFirstObjectByType<EnergySystem>();

            jugadorDentro = true;

            // Activa el efecto de la viñeta roja en la pantalla
            if (VignetteUI.Instance != null)
            {
                VignetteUI.Instance.SetDangerState(true);
            }
        }
    }

    private void Update()
    {
        // Mientras el jugador permanezca dentro de la zona, le quitamos energía continuamente
        if (jugadorDentro && energySystem != null)
        {
            energySystem.currentEnergy -= dañoEnergiaPorSegundo * Time.deltaTime;
            energySystem.currentEnergy = Mathf.Clamp(energySystem.currentEnergy, 0, energySystem.maxEnergy);

            if (energySystem.currentEnergy <= 0)
            {
                energySystem.PerderJuego();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SalirDeZona();
        }
    }

    private void OnDisable()
    {
        // Si la zona se apaga sola mientras el jugador sigue dentro
        SalirDeZona();
    }

    private void SalirDeZona()
    {
        jugadorDentro = false;

        // Desactiva la viñeta roja de la pantalla
        if (VignetteUI.Instance != null)
        {
            VignetteUI.Instance.SetDangerState(false);
        }
    }
}