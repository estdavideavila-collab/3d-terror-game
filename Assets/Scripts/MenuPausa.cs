using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject panelPausa;
    public MenuController menuController;
    public EnergySystem energySystem;
    public PlayerController playerController;

    [Header("Sonido de pausa (suena en bucle mientras el juego está pausado)")]
    public AudioClip sonidoPausa;
    [Range(0f, 1f)] public float volumenPausa = 0.5f;
    public float fadeInPausa = 0.3f;    // segundos que tarda en aparecer al pausar
    public float fadeOutPausa = 0.4f;   // segundos que tarda en desvanecerse al reanudar

    [Header("Opciones")]
    [Tooltip("Actívalo solo si Esc no responde al probar dentro del editor de Unity.")]
    public bool usarTambienTeclaP = false;

    private bool pausado = false;
    private AudioSource fuentePausa;
    private float nivelPausa = 0f; // 0 = silencio, 1 = volumen completo

    void Start()
    {
        if (panelPausa != null) panelPausa.SetActive(false);

        // Si se te olvidó arrastrar alguna referencia, las busca solo
        if (menuController == null) menuController = FindFirstObjectByType<MenuController>();
        if (energySystem == null) energySystem = FindFirstObjectByType<EnergySystem>();
        if (playerController == null) playerController = FindFirstObjectByType<PlayerController>();

        // AudioSource propio del sonido de pausa
        fuentePausa = gameObject.AddComponent<AudioSource>();
        fuentePausa.clip = sonidoPausa;
        fuentePausa.loop = true;                  // suena sin parar mientras dure la pausa
        fuentePausa.playOnAwake = false;
        fuentePausa.spatialBlend = 0f;            // 2D
        fuentePausa.volume = 0f;
        fuentePausa.ignoreListenerPause = true;   // se oye aunque el resto del audio esté pausado
        fuentePausa.ignoreListenerVolume = true;

        if (sonidoPausa != null) sonidoPausa.LoadAudioData();
    }

    void Update()
    {
        ActualizarSonidoPausa();

        if (Keyboard.current == null) return;

        bool tecla = Keyboard.current.escapeKey.wasPressedThisFrame ||
                     (usarTambienTeclaP && Keyboard.current.pKey.wasPressedThisFrame);
        if (!tecla) return;

        if (pausado) Reanudar();
        else if (PuedePausar()) Pausar();
    }

    // Fundido del sonido de pausa (tiempo real, porque con la pausa Time.timeScale vale 0)
    void ActualizarSonidoPausa()
    {
        if (sonidoPausa == null || fuentePausa == null) return;

        float objetivo = pausado ? 1f : 0f;
        float duracion = pausado ? fadeInPausa : fadeOutPausa;
        nivelPausa = Mathf.MoveTowards(nivelPausa, objetivo, Time.unscaledDeltaTime / Mathf.Max(0.01f, duracion));
        fuentePausa.volume = volumenPausa * nivelPausa;

        if (pausado && !fuentePausa.isPlaying) fuentePausa.Play();
        if (!pausado && nivelPausa <= 0f && fuentePausa.isPlaying) fuentePausa.Stop();
    }

    // Solo se puede pausar durante la partida (no en el menú, la cinemática ni el Game Over)
    bool PuedePausar()
    {
        if (energySystem == null || !energySystem.juegoIniciado) return false;
        if (menuController == null || menuController.uiGameplay == null) return false;

        // La interfaz de juego (barra y cronómetro) solo está visible mientras se juega:
        // se oculta en el menú, en la cinemática y al perder
        return menuController.uiGameplay.activeInHierarchy;
    }

    public void Pausar()
    {
        pausado = true;

        Time.timeScale = 0f;        // congela el juego
        AudioListener.pause = true; // congela los sonidos del juego (el de pausa y los de botones no)

        if (panelPausa != null) panelPausa.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Botón REANUDAR
    public void Reanudar()
    {
        pausado = false;

        if (panelPausa != null) panelPausa.SetActive(false);

        AudioListener.pause = false;
        Time.timeScale = 1f;

        if (playerController != null)
        {
            playerController.BloquearCursor();
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Botón MENÚ PRINCIPAL
    public void VolverAlMenuPrincipal()
    {
        pausado = false;

        if (fuentePausa != null) fuentePausa.Stop(); // aquí se corta directo, la escena se recarga

        // Deja todo en estado normal antes de recargar
        AudioListener.pause = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnDestroy()
    {
        // Seguridad: si se sale del Play estando en pausa, no deja nada congelado
        if (pausado)
        {
            AudioListener.pause = false;
            Time.timeScale = 1f;
        }
    }
}