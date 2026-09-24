using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Velocidades de Movimiento")]
    public float walkSpeed = 2.5f;     // Velocidad de caminata humana realista
    public float runSpeed = 5.0f;      // Velocidad al correr manteniendo Shift

    [Header("Cámara")]
    public float mouseSensitivity = 0.05f;
    public Transform cameraTransform;

    [Header("Gravedad")]
    public float gravity = -9.81f;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;

    // Variables públicas para que EnergySystem las lea correctamente
    [HideInInspector] public bool IsMoving = false;
    [HideInInspector] public bool IsRunning = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (controller == null) return;

        // 1. LECTURA DE TECLAS WASD
        Vector2 moveInput = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) moveInput.y += 1f;
            if (Keyboard.current.sKey.isPressed) moveInput.y -= 1f;
            if (Keyboard.current.aKey.isPressed) moveInput.x -= 1f;
            if (Keyboard.current.dKey.isPressed) moveInput.x += 1f;
        }

        // Determinar si hay movimiento
        IsMoving = moveInput.magnitude > 0.1f;

        // Detectar si se mantiene presionado Shift para correr
        bool shiftPressed = Keyboard.current != null && 
                           (Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed);

        IsRunning = IsMoving && shiftPressed;

        // Determinar velocidad actual
        float currentSpeed = IsRunning ? runSpeed : walkSpeed;

        // Normalizar entrada diagonal
        if (moveInput.magnitude > 1f)
        {
            moveInput.Normalize();
        }

        // Aplicar movimiento
        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;

        Vector3 finalMotion = (moveDirection * currentSpeed) + velocity;
        controller.Move(finalMotion * Time.deltaTime);

        // 2. CONTROL DE CÁMARA CON EL RATÓN
        if (Mouse.current != null && cameraTransform != null)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity;

            xRotation -= mouseDelta.y;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseDelta.x);
        }
    }
}