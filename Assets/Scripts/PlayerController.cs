using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Velocidades de Movimiento")]
    public float walkSpeed = 2.5f;
    public float runSpeed = 5.0f;

    [Header("Cámara")]
    public float mouseSensitivity = 0.05f;
    public Transform cameraTransform;

    [Header("Gravedad")]
    public float gravity = -9.81f;

    [Header("Salto")]
    public float jumpHeight = 1.2f;

    [Header("Sonido de caída")]
    public AudioClip sonidoCaida;
    [Range(0f, 1f)] public float volumenCaida = 1f;
    public float tiempoMinimoEnAire = 0.25f;     // evita que suene en bajadas pequeñas
    public float distanciaAnticipacion = 0.35f;  // metros antes de tocar el suelo en que suena

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;

    private bool estabaEnSuelo = true;
    private float tiempoEnAire = 0f;
    private bool caidaYaSono = false;
    private AudioSource fuenteCaida;

    [HideInInspector] public bool IsMoving = false;
    [HideInInspector] public bool IsRunning = false;
    [HideInInspector] public bool EstaEnSuelo = true;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        // AudioSource propio para la caída, en un hijo para no mezclarse con el de los pasos
        GameObject audioCaida = new GameObject("AudioCaida");
        audioCaida.transform.SetParent(transform, false);
        fuenteCaida = audioCaida.AddComponent<AudioSource>();
        fuenteCaida.playOnAwake = false;
        fuenteCaida.loop = false;
        fuenteCaida.spatialBlend = 0f;

        // Deja el audio ya cargado en memoria para que suene sin demora
        if (sonidoCaida != null) sonidoCaida.LoadAudioData();

        DesbloquearCursor();
    }

    void Update()
    {
        if (controller == null) return;

        bool juegoActivo = Cursor.lockState == CursorLockMode.Locked && Time.timeScale > 0f;

        // 1. LECTURA DE TECLAS WASD
        Vector2 moveInput = Vector2.zero;
        if (Keyboard.current != null && juegoActivo)
        {
            if (Keyboard.current.wKey.isPressed) moveInput.y += 1f;
            if (Keyboard.current.sKey.isPressed) moveInput.y -= 1f;
            if (Keyboard.current.aKey.isPressed) moveInput.x -= 1f;
            if (Keyboard.current.dKey.isPressed) moveInput.x += 1f;
        }

        IsMoving = moveInput.magnitude > 0.1f;

        bool shiftPressed = juegoActivo && Keyboard.current != null &&
                           (Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed);

        IsRunning = IsMoving && shiftPressed;

        float currentSpeed = IsRunning ? runSpeed : walkSpeed;

        if (moveInput.magnitude > 1f)
        {
            moveInput.Normalize();
        }

        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;

        // 2. SUELO, CAÍDA Y SALTO
        EstaEnSuelo = controller.isGrounded;

        if (EstaEnSuelo && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (!EstaEnSuelo)
        {
            tiempoEnAire += Time.deltaTime;

            // ANTICIPACIÓN: si va cayendo y el suelo está muy cerca, suena ya
            if (!caidaYaSono && juegoActivo && tiempoEnAire >= tiempoMinimoEnAire && velocity.y < 0f)
            {
                float distanciaRayo = distanciaAnticipacion + (-velocity.y * Time.deltaTime);
                Vector3 origen = transform.position + controller.center
                                 + Vector3.down * (controller.height * 0.5f - 0.05f);

                if (Physics.Raycast(origen, Vector3.down, distanciaRayo, ~0, QueryTriggerInteraction.Ignore))
                {
                    ReproducirCaida();
                }
            }
        }
        else
        {
            // Respaldo: si el rayo no llegó a detectarlo, suena al tocar el suelo
            if (!estabaEnSuelo && !caidaYaSono && tiempoEnAire >= tiempoMinimoEnAire && juegoActivo)
            {
                ReproducirCaida();
            }
            tiempoEnAire = 0f;
            caidaYaSono = false;
        }
        estabaEnSuelo = EstaEnSuelo;

        // Saltar con ESPACIO (solo desde el suelo)
        if (EstaEnSuelo && juegoActivo && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        Vector3 finalMotion = (moveDirection * currentSpeed) + velocity;
        controller.Move(finalMotion * Time.deltaTime);

        // 3. CONTROL DE CÁMARA CON EL RATÓN
        if (Cursor.lockState == CursorLockMode.Locked && Mouse.current != null && cameraTransform != null)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity;

            xRotation -= mouseDelta.y;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseDelta.x);
        }
    }

    void ReproducirCaida()
    {
        caidaYaSono = true;
        if (sonidoCaida != null && fuenteCaida != null)
        {
            fuenteCaida.PlayOneShot(sonidoCaida, volumenCaida);
        }
    }

    public void BloquearCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void DesbloquearCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}