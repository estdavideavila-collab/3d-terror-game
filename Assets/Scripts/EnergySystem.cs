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
    public GameObject botonReintentar;
    public Image energyFill;            
    public TMP_Text textoTiempo;        
    public TMP_Text gameOverText;

    [Header("Sonido de Game Over")]
    public AudioClip sonidoGameOver;
    [Range(0f, 1f)] public float volumenGameOver = 1f;

    private AudioSource fuenteGameOver;

    private PlayerController playerController;

    void Start()
    {
        Time.timeScale = 1f;
        currentEnergy = maxEnergy;
        playerController = GetComponent<PlayerController>();

        if (gameOverText != null) gameOverText.gameObject.SetActive(false); if (botonReintentar != null) botonReintentar.SetActive(false);
        if (energyFill != null) energyFill.color = colorNormal;

        // AudioSource propio para el Game Over, en un hijo para no mezclarse con otros sonidos del Player
        GameObject audioGameOver = new GameObject("AudioGameOver");
        audioGameOver.transform.SetParent(transform, false);
        fuenteGameOver = audioGameOver.AddComponent<AudioSource>();
        fuenteGameOver.playOnAwake = false;
        fuenteGameOver.loop = false;
        fuenteGameOver.spatialBlend = 0f; // 2D: se oye igual en ambos oídos

        if (sonidoGameOver != null) sonidoGameOver.LoadAudioData();

        // Si hay un menú activo, el juego empieza pausado hasta pulsar JUGAR
        juegoIniciado = false;
    }

    void Update()
    {
        if (juegoTerminado) return;

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
            estaCaminando = Keyboard.current.wKey.isPressed ||
                            Keyboard.current.aKey.isPressed ||
                            Keyboard.current.sKey.isPressed ||
                            Keyboard.current.dKey.isPressed;

            estaCorriendo = estaCaminando && 
                           (Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed);
        }

        // CÁLCULOS Y CONSUMO DE ENERGÍA (Solo cuando el gameplay está activo)
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

    // Llamado desde el botón JUGAR a través del MenuController
    public void IniciarGameplay()
    {
        juegoIniciado = true;
        juegoTerminado = false;
        
        if (energyFill != null)
        {
            energyFill.fillAmount = currentEnergy / maxEnergy;
            energyFill.color = colorNormal;
        }

        ActualizarTextoTiempo();
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
        if (juegoTerminado) return;
        juegoTerminado = true;

        // 1. Guarda el tiempo sobrevivido en el ranking
        string nombre = string.IsNullOrWhiteSpace(RankingManager.NombreJugador)
            ? "Jugador"
            : RankingManager.NombreJugador;
        RankingManager.Agregar(nombre, tiempoTranscurrido);

        // 2. Muestra el mensaje de Game Over con el tiempo
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.text = "¡HAS PERDIDO!\n<size=40%>Tiempo: " + RankingManager.Formato(tiempoTranscurrido) + "</size>";
        }

        // 3. Sonido de Game Over
        if (sonidoGameOver != null && fuenteGameOver != null)
        {
            fuenteGameOver.PlayOneShot(sonidoGameOver, volumenGameOver);
        }

        // 4. Muestra el botón de reintentar
        if (botonReintentar != null) botonReintentar.SetActive(true);

        // 5. Libera el cursor para poder pulsar el botón
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 6. Oculta la barra de energía y el cronómetro (grupo UIGameplay)
        MenuController menu = FindFirstObjectByType<MenuController>();
        if (menu != null && menu.uiGameplay != null)
            menu.uiGameplay.SetActive(false);

        // 7. Pausa el juego
        Time.timeScale = 0f;
    }
}
