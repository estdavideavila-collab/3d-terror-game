using UnityEngine;
using UnityEngine.SceneManagement;

public class ReintentarJuego : MonoBehaviour
{
    public void Reintentar()
    {
        Time.timeScale = 1f; // el Game Over pausó el juego
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}