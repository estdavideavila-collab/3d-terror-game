using UnityEngine;

public class DangerousZone : MonoBehaviour
{
    [Header("Ajustes de Peligro")]
    [Tooltip("Aparecerá en el Inspector para cambiar la velocidad de drenaje")]
    public float multiplicadorDrenaje = 3.0f;

    private void OnTriggerEnter(Collider other)
    {
        // Detecta al jugador incluso si el collider toca una extremidad o hijo
        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {
            Debug.Log("<color=red>¡JUGADOR ENTRÓ A ZONA PELIGROSA!</color>");

            // Busca el EnergySystem en el jugador
            EnergySystem energy = other.GetComponentInParent<EnergySystem>();
            if (energy == null) energy = FindFirstObjectByType<EnergySystem>();

            if (energy != null)
            {
                energy.multiplicadorZona = multiplicadorDrenaje;
                Debug.Log("Multiplicador cambiado a: " + energy.multiplicadorZona);
            }
            else
            {
                Debug.LogError("No se encontró el script EnergySystem en el Player.");
            }

            // Efectos opcionales de UI/Niebla si los usas
            VignetteUI vignette = FindFirstObjectByType<VignetteUI>();
            if (vignette != null) vignette.enabled = true;

            NieblaDinamica niebla = FindFirstObjectByType<NieblaDinamica>();
            if (niebla != null) niebla.EntrarEnPeligro();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {
            Debug.Log("<color=green>¡JUGADOR SALIÓ DE ZONA PELIGROSA!</color>");

            EnergySystem energy = other.GetComponentInParent<EnergySystem>();
            if (energy == null) energy = FindFirstObjectByType<EnergySystem>();

            if (energy != null)
            {
                energy.multiplicadorZona = 1f;
            }

            VignetteUI vignette = FindFirstObjectByType<VignetteUI>();
            if (vignette != null) vignette.enabled = false;

            NieblaDinamica niebla = FindFirstObjectByType<NieblaDinamica>();
            if (niebla != null) niebla.SalirDePeligro();
        }
    }
}