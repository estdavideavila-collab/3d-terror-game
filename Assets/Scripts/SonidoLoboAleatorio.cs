using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class SonidoLoboAleatorio : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador; // El Player para calcular distancias a su alrededor

    [Header("Tiempo entre aullidos")]
    public float tiempoMinimo = 10f;
    public float tiempoMaximo = 20f;

    [Header("Distancia del aullido respecto al jugador")]
    public float distanciaMinima = 15f;
    public float distanciaMaxima = 35f;

    [Header("Variaciones de audio")]
    public float tonoMinimo = 0.8f;
    public float tonoMaximo = 1.15f;

    public float volumenMinimo = 0.5f;
    public float volumenMaximo = 1f;

    private AudioSource audioLobo;

    void Start()
    {
        audioLobo = GetComponent<AudioSource>();

        audioLobo.playOnAwake = false;
        audioLobo.loop = false;

        // Si no asignaste el jugador en el inspector, intenta buscarlo automáticamente
        if (jugador == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) jugador = p.transform;
        }

        StartCoroutine(ReproducirAullidos());
    }

    IEnumerator ReproducirAullidos()
    {
        while (true)
        {
            float espera = Random.Range(tiempoMinimo, tiempoMaximo);
            yield return new WaitForSeconds(espera);

            if (audioLobo.clip != null)
            {
                // 1. Mover el emisor de sonido a un punto aleatorio alrededor del jugador
                MoverAuiidoAlrededorDelJugador();

                // 2. Variar tono y volumen aleatoriamente
                audioLobo.pitch = Random.Range(tonoMinimo, tonoMaximo);
                audioLobo.volume = Random.Range(volumenMinimo, volumenMaximo);

                // 3. Reproducir aullido posicional
                audioLobo.Play();

                // 4. Esperar hasta que termine
                yield return new WaitWhile(() => audioLobo.isPlaying);
            }
        }
    }

    void MoverAuiidoAlrededorDelJugador()
    {
        if (jugador == null) return;

        // Generar una dirección aleatoria en un círculo horizontal 2D (XZ)
        Vector2 direccionAleatoria = Random.insideUnitCircle.normalized;
        float distanciaAleatoria = Random.Range(distanciaMinima, distanciaMaxima);

        // Calcular nueva posición en el mundo alrededor del jugador
        Vector3 nuevaPosicion = jugador.position + new Vector3(direccionAleatoria.x, 0, direccionAleatoria.y) * distanciaAleatoria;

        // Mover este GameObject a la nueva ubicación
        transform.position = nuevaPosicion;
    }
}