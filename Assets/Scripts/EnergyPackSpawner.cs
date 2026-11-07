using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyPackSpawner : MonoBehaviour
{
    [Header("Prefabs de los Ítems (Botella y Manzana)")]
    public GameObject[] energyPackPrefabs;

    [Header("Control de Cantidad")]
    public int maxPacksSimultaneos = 8;
    public float tiempoComprobacion = 2f;

    [Header("Área de Spawn")]
    public Vector3 areaCentro = Vector3.zero;
    public Vector3 areaTamano = new Vector3(60f, 0f, 60f);

    [Header("Ajuste de Suelo (Raycast)")]
    public float alturaDeteccionRayo = 100f;
    public float elevacionSobreSuelo = 1.4f;  // ~ altura de la vista del jugador
    public LayerMask capaSuelo;

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

            while (packsActivos.Count < maxPacksSimultaneos &&
                   energyPackPrefabs != null && energyPackPrefabs.Length > 0)
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

        Vector3 origenRayo = new Vector3(posX, areaCentro.y + alturaDeteccionRayo, posZ);
        Vector3 posicionFinal;

        if (Physics.Raycast(origenRayo, Vector3.down, out RaycastHit hit, alturaDeteccionRayo * 2f, capaSuelo))
        {
            posicionFinal = hit.point + new Vector3(0f, elevacionSobreSuelo, 0f);
        }
        else
        {
            posicionFinal = new Vector3(posX, areaCentro.y + elevacionSobreSuelo, posZ);
        }

        // Elige botella o manzana al azar
        GameObject prefab = energyPackPrefabs[Random.Range(0, energyPackPrefabs.Length)];
        GameObject nuevoPack = Instantiate(prefab, posicionFinal, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
        packsActivos.Add(nuevoPack);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(areaCentro, areaTamano);
    }
}