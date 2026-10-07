using UnityEngine;

public class EnergyPack : MonoBehaviour
{
    [Header("Ajustes de Recarga")]
    public float energiaAportada = 5f; // Cantidad fija de energía a recuperar

    private bool yaFueRecogido = false; // Evita que se active múltiples veces en el mismo frame

    private void OnTriggerEnter(Collider other)
    {
        if (yaFueRecogido) return; // Si ya se procesó, ignora cualquier otra colisión

        if (other.CompareTag("Player"))
        {
            EnergySystem energySystem = other.GetComponent<EnergySystem>();
            if (energySystem == null) energySystem = FindFirstObjectByType<EnergySystem>();

            if (energySystem != null)
            {
                yaFueRecogido = true; // Bloquea recargas extra inmediatas

                // Suma únicamente la cantidad asignada
                energySystem.currentEnergy += energiaAportada;
                energySystem.currentEnergy = Mathf.Clamp(energySystem.currentEnergy, 0f, energySystem.maxEnergy);

                // Destruye el objeto inmediatamente
                Destroy(gameObject);
            }
        }
    }
}