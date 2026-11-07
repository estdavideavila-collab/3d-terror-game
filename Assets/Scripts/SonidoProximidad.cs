using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SonidoProximidad : MonoBehaviour
{
    [Header("Configuración de Sonido")]
    public AudioClip sonidoPulso;
    [Range(0f, 1f)] public float volumenMaximo = 0.6f;

    [Header("Ritmo Constante (segundos)")]
    public float intervaloFijo = 1.0f;

    [Header("Distancias de Audio 3D (metros)")]
    public float distanciaMinima = 1.5f;
    public float distanciaMaxima = 14.0f;

    // Lista compartida por TODOS los objetos con este script (manzanas, botellas, etc.)
    private static readonly List<SonidoProximidad> todos = new List<SonidoProximidad>();
    private static SonidoProximidad masCercano;
    private static int frameCalculado = -1;

    private AudioSource fuente;
    private Transform oyente;
    private float temporizador;
    private bool sonando;

    void Awake()
    {
        fuente = GetComponent<AudioSource>();
        fuente.playOnAwake = false;
        fuente.loop = false;
        fuente.spatialBlend = 1f;
        fuente.rolloffMode = AudioRolloffMode.Linear;
        fuente.minDistance = distanciaMinima;
        fuente.maxDistance = distanciaMaxima;
        fuente.pitch = 1f;
        fuente.volume = volumenMaximo;
    }

    void OnEnable()  { if (!todos.Contains(this)) todos.Add(this); }
    void OnDisable() { todos.Remove(this); }
    void OnDestroy() { todos.Remove(this); }

    // Llámalo desde el script de recoger si NO destruyes ni desactivas el objeto
    public void Recogido()
    {
        todos.Remove(this);
        fuente.Stop();
        enabled = false;
        frameCalculado = -1; // fuerza recalcular el más cercano en el siguiente frame
    }

    void Update()
    {
        if (sonidoPulso == null) return;
        if (Cursor.lockState != CursorLockMode.Locked || Time.timeScale <= 0f) return;

        if (oyente == null)
        {
            if (Camera.main == null) return;
            oyente = Camera.main.transform;
        }

        CalcularMasCercano();

        // Si no soy el más cercano, me callo y reinicio
        if (masCercano != this)
        {
            if (sonando) { fuente.Stop(); sonando = false; }
            return;
        }

        // Soy el más cercano
        if (!sonando)
        {
            sonando = true;
            temporizador = intervaloFijo; // suena casi de inmediato
        }

        float distancia = Vector3.Distance(transform.position, oyente.position);
        if (distancia > distanciaMaxima)
        {
            temporizador = intervaloFijo;
            return;
        }

        temporizador += Time.deltaTime;
        if (temporizador >= intervaloFijo)
        {
            temporizador = 0f;
            fuente.PlayOneShot(sonidoPulso);
        }
    }

    // Se ejecuta una sola vez por frame, sin importar cuántos objetos haya
    static void CalcularMasCercano()
    {
        if (frameCalculado == Time.frameCount) return;
        frameCalculado = Time.frameCount;

        masCercano = null;
        float menor = float.MaxValue;
        Transform cam = Camera.main != null ? Camera.main.transform : null;
        if (cam == null) return;

        for (int i = todos.Count - 1; i >= 0; i--)
        {
            var o = todos[i];
            if (o == null || !o.enabled || !o.gameObject.activeInHierarchy)
            {
                todos.RemoveAt(i);
                continue;
            }
            float d = (o.transform.position - cam.position).sqrMagnitude;
            if (d < menor)
            {
                menor = d;
                masCercano = o;
            }
        }
    }
}