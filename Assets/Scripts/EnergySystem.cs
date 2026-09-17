using UnityEngine;
using UnityEngine.SceneManagement;

public class EnergySystem : MonoBehaviour
{
    [Header("Energia")]
    public float maxEnergy = 100f;
    public float currentEnergy = 100f;

    [Header("Consumo")]
    public float energyDrainPerSecond = 2f;

    private bool isDead = false;

    void Start()
    {
        currentEnergy = maxEnergy;
    }

    void Update()
    {
        if (isDead)
            return;

        DrainEnergy();

        if (currentEnergy <= 0)
        {
            Die();
        }
    }

    void DrainEnergy()
    {
        currentEnergy -= energyDrainPerSecond * Time.deltaTime;

        currentEnergy = Mathf.Clamp(
            currentEnergy,
            0f,
            maxEnergy
        );

        Debug.Log("Energia: " + currentEnergy);
    }

    void Die()
    {
        isDead = true;

        Debug.Log("El jugador murio");

        Time.timeScale = 0f;
    }
}
