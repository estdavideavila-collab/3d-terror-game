
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Velocidades de Movimiento")]
    public float walkSpeed = 2.5f;
    public float runSpeed = 5.0f;

    [Header("Camara")]
    public float mouseSensitivity = 0.05f;
    public Transform cameraTransform;

    [Header("Salto y Gravedad")]
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;

    [HideInInspector] public bool IsMoving = false;
    [HideInInspector] public bool IsRunning = false;
    [HideInInspector] public bool PuedeCorrer = true;

    // Utilizado por SonidoPasos
    public bool EstaEnSuelo =>
        controller != null && controller.isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        PuedeCorrer = true;

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        DesbloquearCursor();
    }

    void Update()
    {
        if (controller == null) return;

        bool jugando =
            Cursor.lockState == CursorLockMode.Locked &&
            Time.timeScale > 0f;

        Vector2 moveInput = Vector2.zero;

        if (jugando && Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed)
                moveInput.y += 1f;

            if (Keyboard.current.sKey.isPressed)
                moveInput.y -= 1f;

            if (Keyboard.current.aKey.isPressed)
                moveInput.x -= 1f;

            if (Keyboard.current.dKey.isPressed)
                moveInput.x += 1f;
        }

        IsMoving = moveInput.magnitude > 0.1f;

        bool shiftPressed =
            jugando &&
            Keyboard.current != null &&
            (Keyboard.current.leftShiftKey.isPressed ||
             Keyboard.current.rightShiftKey.isPressed);

        IsRunning = IsMoving && shiftPressed && PuedeCorrer;

        float currentSpeed = IsRunning ? runSpeed : walkSpeed;

        if (moveInput.magnitude > 1f)
            moveInput.Normalize();

        Vector3 moveDirection =
            transform.right * moveInput.x +
            transform.forward * moveInput.y;

        // Detectar contacto con el suelo
        if (controller.isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        // SALTO CON ESPACIO
        if (jugando &&
            controller.isGrounded &&
            Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            velocity.y =
                Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Aplicar gravedad
        velocity.y += gravity * Time.deltaTime;

        Vector3 finalMotion =
            moveDirection * currentSpeed + velocity;

        controller.Move(finalMotion * Time.deltaTime);

        // Movimiento de camara
        if (jugando &&
            Mouse.current != null &&
            cameraTransform != null)
        {
            Vector2 mouseDelta =
                Mouse.current.delta.ReadValue() * mouseSensitivity;

            xRotation -= mouseDelta.y;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            cameraTransform.localRotation =
                Quaternion.Euler(xRotation, 0f, 0f);

            transform.Rotate(Vector3.up * mouseDelta.x);
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
