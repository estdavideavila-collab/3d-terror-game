using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SonidoProximidad : MonoBehaviour
{
    [Header("Sonido")]
    public AudioClip sonidoPulso;          // sonido CORTO (menos de 0.5 s)
    [Range(0f, 1f)] public float volumen = 1f;

    [Header("Distancias (metros)")]
    public float distanciaMinima = 1.5f;   // dentro de esta distancia suena al máximo
    public float distanciaMaxima = 14f;    // más lejos de esto no se oye

    [Header("Ritmo de los pulsos (segundos entre pulso y pulso)")]
    public float intervaloCerca = 0.3f;
    public float intervaloLejos = 1.6f;

    [Header("Tono (pitch)")]
    public float tonoCerca = 1.3f;
    public float tonoLejos = 0.8f;

    [Header("Opciones")]
    public bool soloElMasCercano = true;   // solo suena el objeto más cercano al jugador

    private static readonly List<SonidoProximidad> todos = new List<SonidoProximidad>();

    private AudioSource fuente;
    private Transform oyente;
    private float temporizador;

    void Awake()
    {
        fuente = GetComponent<AudioSource>();
        fuente.playOnAwake = false;
        fuente.loop = false;
        fuente.spatialBlend = 1f;                    // 3D: da dirección izquierda/derecha
        fuente.rolloffMode = AudioRolloffMode.Linear; // el volumen baja con la distancia
        fuente.minDistance = distanciaMinima;
        fuente.maxDistance = distanciaMaxima;
        fuente.spread = 0f;
        fuente.dopplerLevel = 0f;
    }

    void OnEnable() { todos.Add(this); }
    void OnDisable() { todos.Remove(this); }

    void Update()
    {
        if (sonidoPulso == null) return;

        // Solo suena cuando el juego está activo (no en el menú, la cinemática ni el Game Over)
        if (Cursor.lockState != CursorLockMode.Locked || Time.timeScale <= 0f) return;

        if (oyente == null)
        {
            if (Camera.main == null) return;
            oyente = Camera.main.transform;
        }

        float distancia = Vector3.Distance(transform.position, oyente.position);

        if (distancia > distanciaMaxima)
        {
            temporizador = 0f;
            return;
        }

        if (soloElMasCercano && !EsElMasCercano(distancia)) return;

        // 0 = muy cerca, 1 = muy lejos
        float t = Mathf.InverseLerp(distanciaMinima, distanciaMaxima, distancia);
        float intervalo = Mathf.Lerp(intervaloCerca, intervaloLejos, t);

        temporizador += Time.deltaTime;
        if (temporizador >= intervalo)
        {
            temporizador = 0f;
            fuente.pitch = Mathf.Lerp(tonoCerca, tonoLejos, t);
            fuente.PlayOneShot(sonidoPulso, volumen);
        }
    }

    bool EsElMasCercano(float miDistancia)
    {
        foreach (SonidoProximidad otro in todos)
        {
            if (otro == this) continue;
            if (Vector3.Distance(otro.transform.position, oyente.position) < miDistancia)
                return false;
        }
        return true;
    }
}