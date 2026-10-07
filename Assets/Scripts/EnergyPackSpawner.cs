using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyPackSpawner : MonoBehaviour
{
    [Header("Prefab del Ítem")]
    public GameObject energyPackPrefab;

    [Header("Control de Cantidad")]
    public int maxPacksSimultaneos = 8;
    public float tiempoComprobacion = 2f;

    [Header("Área de Spawn")]
    public Vector3 areaCentro = Vector3.zero;
    public Vector3 areaTamano = new Vector3(60f, 0f, 60f);
    
    [Header("Ajuste de Suelo (Raycast)")]
    public float alturaDeteccionRayo = 100f; // Altura desde la que dispara el rayo hacia abajo
    public float elevacionSobreSuelo = 0.8f;  // Cuánto flota la esfera sobre el pasto/suelo
    public LayerMask capaSuelo;               // Capa para detectar solo el terreno

    private List<GameObject> packsActivos = new List<GameObject>();

    void Start()
    {
        StartCoroutine(RutinaGenerarPacks());
    }

    IEnumerator RutinaGenerarPacks()
    {
        while (true)
        {
            packsActivos.RemoveAll(pack => pack == null);

            while (packsActivos.Count < maxPacksSimultaneos && energyPackPrefab != null)
            {
                CrearNuevoPack();
            }

            yield return new WaitForSeconds(tiempoComprobacion);
        }
    }

    void CrearNuevoPack()
    {
        float posX = Random.Range(areaCentro.x - (areaTamano.x / 2f), areaCentro.x + (areaTamano.x / 2f));
        float posZ = Random.Range(areaCentro.z - (areaTamano.z / 2f), areaCentro.z + (areaTamano.z / 2f));

        // Punto de origen del rayo en el aire
        Vector3 origenRayo = new Vector3(posX, areaCentro.y + alturaDeteccionRayo, posZ);
        Vector3 posicionFinal = Vector3.zero;

        // Dispara un rayo verticalmente hacia abajo buscando el suelo
        if (Physics.Raycast(origenRayo, Vector3.down, out RaycastHit hit, alturaDeteccionRayo * 2f, capaSuelo))
        {
            // Detectó el terreno: coloca el ítem a la altura exacta del impacto + elevación
            posicionFinal = hit.point + new Vector3(0f, elevacionSobreSuelo, 0f);
        }
        else
        {
            // Si por alguna razón no choca con el suelo, usa una altura base por defecto
            posicionFinal = new Vector3(posX, areaCentro.y + elevacionSobreSuelo, posZ);
        }

        GameObject nuevoPack = Instantiate(energyPackPrefab, posicionFinal, Quaternion.identity);
        packsActivos.Add(nuevoPack);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(areaCentro, areaTamano);
    }
}