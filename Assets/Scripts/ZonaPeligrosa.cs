using UnityEngine;

public class ZonaPeligrosa : MonoBehaviour
{
    [Header("Ajustes de Peligro")]
    [Tooltip("Multiplicador del consumo de energía cuando el jugador está dentro (ej. 3.0 multiplica x3 la pérdida).")]
    public float multiplicadorDrenaje = 3.0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("<color=red>¡ENTRÓ A ZONA DE PELIGRO!</color>");

            // 1. Activar viñeta de peligro UI
            VignetteUI vignette = FindFirstObjectByType<VignetteUI>();
            if (vignette != null) vignette.enabled = true;

            // 2. Aumentar la velocidad del consumo de energía
            EnergySystem energy = other.GetComponent<EnergySystem>();
            if (energy == null) energy = FindFirstObjectByType<EnergySystem>();

            if (energy != null)
            {
                energy.multiplicadorZona = multiplicadorDrenaje;
                Debug.Log("Multiplicador de energía activado: " + energy.multiplicadorZona);
            }

            // 3. Intensificar niebla
            NieblaDinamica niebla = FindFirstObjectByType<NieblaDinamica>();
            if (niebla != null) niebla.EntrarEnPeligro();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("<color=green>¡SALIÓ DE LA ZONA DE PELIGRO!</color>");

            // 1. Desactivar viñeta UI
            VignetteUI vignette = FindFirstObjectByType<VignetteUI>();
            if (vignette != null) vignette.enabled = false;

            // 2. Restaurar consumo normal de energía
            EnergySystem energy = other.GetComponent<EnergySystem>();
            if (energy == null) energy = FindFirstObjectByType<EnergySystem>();

            if (energy != null)
            {
                energy.multiplicadorZona = 1f;
            }

            // 3. Restaurar niebla normal
            NieblaDinamica niebla = FindFirstObjectByType<NieblaDinamica>();
            if (niebla != null) niebla.SalirDePeligro();
        }
    }

    private void OnDestroy()
    {
        // Limpieza de seguridad si el objeto de la zona se destruye durante la partida
        VignetteUI vignette = FindFirstObjectByType<VignetteUI>();
        if (vignette != null) vignette.enabled = false;

        NieblaDinamica niebla = FindFirstObjectByType<NieblaDinamica>();
        if (niebla != null) niebla.SalirDePeligro();

        EnergySystem energy = FindFirstObjectByType<EnergySystem>();
        if (energy != null) energy.multiplicadorZona = 1f;
    }
}