
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlLinterna : MonoBehaviour
{
    [Header("Configuración")]
    public Light luzLinterna;
    public Key tecla = Key.F;

    void Start()
    {
        if (luzLinterna == null)
            luzLinterna = GetComponent<Light>();

        // La linterna comienza apagada
        if (luzLinterna != null)
            luzLinterna.enabled = false;
    }

    void Update()
    {
        // Solo permitir el uso durante el juego
        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        if (Keyboard.current != null &&
            Keyboard.current[tecla].wasPressedThisFrame)
        {
            if (luzLinterna != null)
                luzLinterna.enabled = !luzLinterna.enabled;
        }
    }
}
