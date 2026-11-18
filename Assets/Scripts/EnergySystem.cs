
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class EnergySystem : MonoBehaviour
{
    [Header("Ajustes de Energia")]
    public float maxEnergy = 100f;
    public float currentEnergy = 100f;

    [Header("Tasas de Consumo")]
    public float idleEnergyDrainPerSecond = 1f;
    public float walkEnergyDrainPerSecond = 2f;
    public float runEnergyMultiplier = 2f;

    // Multiplicador de consumo segun la zona
    public float multiplicadorZona = 1f;

    [Header("Colores de la Barra")]
    public Color colorNormal = Color.green;
    public Color colorCritico = Color.red;

    [Header("Ajustes de Tiempo")]
    public float tiempoTranscurrido = 0f;
    public bool juegoIniciado = false;
    private bool juegoTerminado = false;

    [Header("UI")]
    public GameObject botonReintentar;
    public GameObject botonMenuPrincipal;

    public Image energyFill;

    // NUEVO: Slider que controla el recorrido de la calavera
    public Slider barraEnergia;

    public TMP_Text textoTiempo;
    public TMP_Text gameOverText;

    [Header("Sonido de Game Over")]
    public AudioClip sonidoGameOver;

    [Range(0f, 1f)]
    public float volumenGameOver = 1f;

    private AudioSource fuenteGameOver;
    private PlayerController playerController;

    void Start()
    {
        Time.timeScale = 1f;
        currentEnergy = maxEnergy;

        playerController = GetComponent<PlayerController>();

        // Ocultar elementos de Game Over al iniciar
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);

        if (botonReintentar != null)
            botonReintentar.SetActive(false);

        if (botonMenuPrincipal != null)
            botonMenuPrincipal.SetActive(false);

        // Inicializar la barra de energia y el Slider
        ActualizarBarraEnergia();

        // Crear una fuente de audio exclusiva para Game Over
        GameObject audioGameOver = new GameObject("AudioGameOver");

        audioGameOver.transform.SetParent(transform, false);

        fuenteGameOver = audioGameOver.AddComponent<AudioSource>();

        fuenteGameOver.playOnAwake = false;
        fuenteGameOver.loop = false;
        fuenteGameOver.spatialBlend = 0f;

        if (sonidoGameOver != null)
            sonidoGameOver.LoadAudioData();

        // Esperar hasta que comience el gameplay
        juegoIniciado = false;
    }

    void Update()
    {
        if (juegoTerminado)
            return;

        bool estaCaminando = false;
        bool estaCorriendo = false;

        // Detectar movimiento del jugador
        if (playerController != null)
        {
            estaCaminando = playerController.IsMoving;
            estaCorriendo = playerController.IsRunning;
        }
        else if (Keyboard.current != null)
        {
            estaCaminando =
                Keyboard.current.wKey.isPressed ||
                Keyboard.current.aKey.isPressed ||
                Keyboard.current.sKey.isPressed ||
                Keyboard.current.dKey.isPressed;

            estaCorriendo = estaCaminando &&
                (Keyboard.current.leftShiftKey.isPressed ||
                 Keyboard.current.rightShiftKey.isPressed);
        }

        // Consumo de energia durante el gameplay
        if (juegoIniciado)
        {
            tiempoTranscurrido += Time.deltaTime;

            ActualizarTextoTiempo();

            float consumoBase = idleEnergyDrainPerSecond;

            if (estaCaminando)
            {
                float factorCarrera =
                    estaCorriendo ? runEnergyMultiplier : 1f;

                consumoBase +=
                    walkEnergyDrainPerSecond * factorCarrera;
            }

            // Multiplicador de consumo segun la zona
            float factorZona = Mathf.Max(1f, multiplicadorZona);

            currentEnergy -=
                consumoBase * factorZona * Time.deltaTime;

            currentEnergy = Mathf.Clamp(
                currentEnergy,
                0f,
                maxEnergy
            );

            if (currentEnergy <= 0f)
            {
                PerderJuego();
            }
        }

        // NUEVO: mantener el Slider sincronizado
        ActualizarBarraEnergia();
    }

    // NUEVO: actualiza el relleno y el Slider
    void ActualizarBarraEnergia()
    {
        float energiaNormalizada = maxEnergy > 0f
            ? Mathf.Clamp01(currentEnergy / maxEnergy)
            : 0f;

        // Actualizar Slider
        if (barraEnergia != null)
        {
            barraEnergia.minValue = 0f;
            barraEnergia.maxValue = Mathf.Max(1f, maxEnergy);

            barraEnergia.SetValueWithoutNotify(
                Mathf.Clamp(currentEnergy, 0f, barraEnergia.maxValue)
            );
        }

        // Actualizar relleno y color
        if (energyFill != null)
        {
            energyFill.fillAmount = energiaNormalizada;

            energyFill.color =
                currentEnergy <= maxEnergy * 0.3f
                ? colorCritico
                : colorNormal;
        }
    }

    public void IniciarGameplay()
    {
        juegoIniciado = true;
        juegoTerminado = false;

        ActualizarBarraEnergia();
        ActualizarTextoTiempo();
    }

    public void RecargarEnergia(float cantidad)
    {
        currentEnergy = Mathf.Clamp(
            currentEnergy + cantidad,
            0f,
            maxEnergy
        );

        ActualizarBarraEnergia();
    }

    void ActualizarTextoTiempo()
    {
        if (textoTiempo != null)
        {
            int minutos =
                Mathf.FloorToInt(tiempoTranscurrido / 60f);

            int segundos =
                Mathf.FloorToInt(tiempoTranscurrido % 60f);

            textoTiempo.text = string.Format(
                "{0:00}:{1:00}",
                minutos,
                segundos
            );
        }
    }

    public void PerderJuego()
    {
        if (juegoTerminado)
            return;

        juegoTerminado = true;

        // 1. Guardar record solamente si tiene nombre
        if (!string.IsNullOrWhiteSpace(RankingManager.NombreJugador))
        {
            RankingManager.Agregar(
                RankingManager.NombreJugador,
                tiempoTranscurrido
            );
        }

        // 2. Mostrar Game Over y tiempo sobrevivido
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);

            gameOverText.text =
                "¡HAS PERDIDO!\n<size=40%>Tiempo: " +
                RankingManager.Formato(tiempoTranscurrido) +
                "</size>";
        }

        // 3. Reproducir sonido de Game Over
        if (sonidoGameOver != null &&
            fuenteGameOver != null)
        {
            fuenteGameOver.PlayOneShot(
                sonidoGameOver,
                volumenGameOver
            );
        }

        // 4. Mostrar ambos botones
        if (botonReintentar != null)
            botonReintentar.SetActive(true);

        if (botonMenuPrincipal != null)
            botonMenuPrincipal.SetActive(true);

        // 5. Liberar cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 6. Ocultar interfaz del gameplay
        MenuController menu =
            FindFirstObjectByType<MenuController>();

        if (menu != null &&
            menu.uiGameplay != null)
        {
            menu.uiGameplay.SetActive(false);
        }

        // 7. Pausar juego
        Time.timeScale = 0f;
    }
}
