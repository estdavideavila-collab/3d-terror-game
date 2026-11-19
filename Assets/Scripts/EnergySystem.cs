
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

    // Slider que controla la barra y la calavera
    public Slider barraEnergia;

    public TMP_Text textoTiempo;
    public TMP_Text gameOverText;

    [Header("Sonido de Energia Critica")]
    public AudioSource sonidoEnergiaCritica;

    [Range(0f, 1f)]
    public float umbralSonidoCritico = 0.35f;

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

        // Aplicar dificultad seleccionada
        AplicarDificultad();

        // Ocultar elementos de Game Over
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);

        if (botonReintentar != null)
            botonReintentar.SetActive(false);

        if (botonMenuPrincipal != null)
            botonMenuPrincipal.SetActive(false);

        // Preparar sonido de energia critica
        if (sonidoEnergiaCritica != null)
        {
            sonidoEnergiaCritica.playOnAwake = false;
            sonidoEnergiaCritica.loop = true;
            sonidoEnergiaCritica.spatialBlend = 0f;
            sonidoEnergiaCritica.Stop();
        }

        // Inicializar barra de energia
        ActualizarBarraEnergia();

        // Crear fuente de audio exclusiva para Game Over
        GameObject audioGameOver =
            new GameObject("AudioGameOver");

        audioGameOver.transform.SetParent(transform, false);

        fuenteGameOver =
            audioGameOver.AddComponent<AudioSource>();

        fuenteGameOver.playOnAwake = false;
        fuenteGameOver.loop = false;
        fuenteGameOver.spatialBlend = 0f;

        if (sonidoGameOver != null)
            sonidoGameOver.LoadAudioData();

        // Esperar hasta que comience la partida
        juegoIniciado = false;
    }

    // Configurar consumo segun dificultad
    void AplicarDificultad()
    {
        idleEnergyDrainPerSecond =
            DificultadManager.ConsumoQuieto;

        walkEnergyDrainPerSecond =
            DificultadManager.ConsumoCaminando;

        runEnergyMultiplier =
            DificultadManager.MultiplicadorCarrera;

        Debug.Log(
            "Dificultad aplicada: " +
            DificultadManager.dificultadActual
        );
    }

    void Update()
    {
        if (juegoTerminado)
            return;

        bool estaCaminando = false;
        bool estaCorriendo = false;

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

        // Consumo durante la partida
        if (juegoIniciado)
        {
            tiempoTranscurrido += Time.deltaTime;

            ActualizarTextoTiempo();

            float consumoBase =
                idleEnergyDrainPerSecond;

            if (estaCaminando)
            {
                float factorCarrera =
                    estaCorriendo
                    ? runEnergyMultiplier
                    : 1f;

                consumoBase +=
                    walkEnergyDrainPerSecond * factorCarrera;
            }

            // Reducir penalizacion de zonas en Facil
            // Mantener penalizacion original en Dificil
            float factorZona = Mathf.Max(
                1f,
                multiplicadorZona *
                (DificultadManager.EsFacil ? 0.5f : 1f)
            );

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

        // Actualizar barra, calavera y sonido critico
        ActualizarBarraEnergia();
    }

    // Mantener sincronizada la barra y la calavera
    void ActualizarBarraEnergia()
    {
        float energiaNormalizada =
            maxEnergy > 0f
            ? Mathf.Clamp01(currentEnergy / maxEnergy)
            : 0f;

        // Actualizar Slider
        if (barraEnergia != null)
        {
            barraEnergia.minValue = 0f;
            barraEnergia.maxValue = Mathf.Max(1f, maxEnergy);

            barraEnergia.SetValueWithoutNotify(
                Mathf.Clamp(
                    currentEnergy,
                    0f,
                    barraEnergia.maxValue
                )
            );
        }

        // Actualizar relleno y color
        if (energyFill != null)
        {
            energyFill.fillAmount = energiaNormalizada;

            energyFill.color =
                energiaNormalizada <= 0.30f
                ? colorCritico
                : colorNormal;
        }

        // NUEVO: activar sonido al llegar al 35 %
        ActualizarSonidoEnergiaCritica(energiaNormalizada);
    }

    // NUEVO: controlar el audio segun la energia
    void ActualizarSonidoEnergiaCritica(float porcentaje)
    {
        if (sonidoEnergiaCritica == null)
            return;

        bool debeSonar =
            juegoIniciado &&
            !juegoTerminado &&
            porcentaje <= umbralSonidoCritico &&
            porcentaje > 0f;

        if (debeSonar)
        {
            // Evitar reiniciar el audio en cada fotograma
            if (!sonidoEnergiaCritica.isPlaying)
            {
                sonidoEnergiaCritica.Play();
            }
        }
        else
        {
            // Detener al recuperar energia
            if (sonidoEnergiaCritica.isPlaying)
            {
                sonidoEnergiaCritica.Stop();
            }
        }
    }

    public void IniciarGameplay()
    {
        // Aplicar dificultad antes de comenzar
        AplicarDificultad();

        // Actualizar cansancio segun el modo seleccionado
        CansancioJugador cansancio =
            GetComponent<CansancioJugador>();

        if (cansancio != null)
            cansancio.AplicarDificultad();

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

        // Detener audio de energia critica
        if (sonidoEnergiaCritica != null)
            sonidoEnergiaCritica.Stop();

        // Guardar record solo si se registro un nombre
        if (!string.IsNullOrWhiteSpace(
            RankingManager.NombreJugador))
        {
            RankingManager.Agregar(
                RankingManager.NombreJugador,
                tiempoTranscurrido
            );
        }

        // Mostrar pantalla de Game Over
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);

            gameOverText.text =
                "¡HAS PERDIDO!\n<size=40%>Tiempo: " +
                RankingManager.Formato(tiempoTranscurrido) +
                "</size>";
        }

        // Reproducir sonido de Game Over
        if (sonidoGameOver != null &&
            fuenteGameOver != null)
        {
            fuenteGameOver.PlayOneShot(
                sonidoGameOver,
                volumenGameOver
            );
        }

        // Mostrar botones
        if (botonReintentar != null)
            botonReintentar.SetActive(true);

        if (botonMenuPrincipal != null)
            botonMenuPrincipal.SetActive(true);

        // Liberar cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Ocultar interfaz de gameplay
        MenuController menu =
            FindFirstObjectByType<MenuController>();

        if (menu != null &&
            menu.uiGameplay != null)
        {
            menu.uiGameplay.SetActive(false);
        }

        // Pausar juego
        Time.timeScale = 0f;
    }

    void OnDisable()
    {
        // Evitar que el audio continue al desactivar el jugador
        if (sonidoEnergiaCritica != null)
            sonidoEnergiaCritica.Stop();
    }
}
