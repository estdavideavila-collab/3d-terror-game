using UnityEngine;

public class GiroFlotante : MonoBehaviour
{
    public float velocidadGiro = 60f;
    public float amplitud = 0.1f;
    public float velocidadFlote = 2f;

    private Vector3 posInicial;

    void Start()
    {
        posInicial = transform.position;
    }

    void Update()
    {
        transform.Rotate(0f, velocidadGiro * Time.deltaTime, 0f, Space.World);
        transform.position = posInicial + Vector3.up * Mathf.Sin(Time.time * velocidadFlote) * amplitud;
    }
}