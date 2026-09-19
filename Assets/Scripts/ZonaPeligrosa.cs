using UnityEngine;

public class ZonaPeligrosa : MonoBehaviour
{
    [Header("Referencias")]
    public EnergySystem sistemaDeEnergia;
    public VinetaDePeligro vinetaDePeligro;

    [Header("Configuracion del peligro")]
    public float multiplicadorDeConsumoEnergia = 3f;

    [Header("Efectos adicionales (opcional)")]
    public GameObject efectoVisualDePeligro;
    public AudioSource sonidoDePeligro;

    void OnTriggerEnter(Collider colisionador)
    {
        if (!colisionador.CompareTag("Player"))
            return;

        if (sistemaDeEnergia != null)
        {
            sistemaDeEnergia.multiplicadorDeConsumo = multiplicadorDeConsumoEnergia;
        }

        if (vinetaDePeligro != null)
        {
            vinetaDePeligro.Mostrar();
        }

        if (efectoVisualDePeligro != null)
        {
            efectoVisualDePeligro.SetActive(true);
        }

        if (sonidoDePeligro != null && !sonidoDePeligro.isPlaying)
        {
            sonidoDePeligro.Play();
        }
    }

    void OnTriggerExit(Collider colisionador)
    {
        if (!colisionador.CompareTag("Player"))
            return;

        if (sistemaDeEnergia != null)
        {
            sistemaDeEnergia.multiplicadorDeConsumo = 1f;
        }

        if (vinetaDePeligro != null)
        {
            vinetaDePeligro.Ocultar();
        }

        if (efectoVisualDePeligro != null)
        {
            efectoVisualDePeligro.SetActive(false);
        }

        if (sonidoDePeligro != null)
        {
            sonidoDePeligro.Stop();
        }
    }
}