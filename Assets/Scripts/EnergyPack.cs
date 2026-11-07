using UnityEngine;

public class EnergyPack : MonoBehaviour
{
    [Header("Ajustes de Recarga")]
    public float energiaAportada = 5f;

    [Header("Sonido al recoger")]
    public AudioClip sonidoMejora;
    [Range(0f, 1f)] public float volumenMejora = 0.5f;

    private bool yaFueRecogido = false;

    private void OnTriggerEnter(Collider other)
    {
        if (yaFueRecogido) return;

        if (other.CompareTag("Player"))
        {
            EnergySystem energySystem = other.GetComponent<EnergySystem>();
            if (energySystem == null) energySystem = FindFirstObjectByType<EnergySystem>();

            if (energySystem != null)
            {
                yaFueRecogido = true;

                energySystem.currentEnergy += energiaAportada;
                energySystem.currentEnergy = Mathf.Clamp(energySystem.currentEnergy, 0f, energySystem.maxEnergy);

                // Sonido de confirmación al recoger (volumen moderado)
                if (sonidoMejora != null)
                {
                    Vector3 posicionSonido = Camera.main != null
                        ? Camera.main.transform.position
                        : transform.position;
                    AudioSource.PlayClipAtPoint(sonidoMejora, posicionSonido, volumenMejora);
                }

                Destroy(gameObject);
            }
        }
    }
}