using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnergySystem : MonoBehaviour
{
    public float maxEnergy = 100f;
    public float currentEnergy = 100f;
    public float energyDrainPerSecond = 20f;

    [Header("UI")]
    public Image energyFill;
    public TMP_Text gameOverText;

    private bool isDead = false;

    void Start()
    {
        Time.timeScale = 1f;

        currentEnergy = maxEnergy;

        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);

        UpdateEnergyBar();
    }

    void Update()
    {
        if (isDead)
            return;

        currentEnergy -= energyDrainPerSecond * Time.deltaTime;

        currentEnergy = Mathf.Clamp(
            currentEnergy,
            0f,
            maxEnergy
        );

        UpdateEnergyBar();

        if (currentEnergy <= 0f)
        {
            Die();
        }
    }

    void UpdateEnergyBar()
    {
        if (energyFill != null)
        {
            energyFill.fillAmount =
                currentEnergy / maxEnergy;
        }
    }

    void Die()
    {
        isDead = true;

        if (gameOverText != null)
            gameOverText.gameObject.SetActive(true);

        Debug.Log("GAME OVER");

        Time.timeScale = 0f;
    }
}
