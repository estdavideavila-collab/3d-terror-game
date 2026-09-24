using System.Collections;
using UnityEngine;

public class ZoneSpawner : MonoBehaviour
{
    public GameObject dangerousZoneObject; // Objeto de la zona con el Trigger

    [Header("Tiempos de Aparición")]
    public float minIntervalo = 10f; // Tiempo mínimo entre zonas
    public float maxIntervalo = 20f; // Tiempo máximo entre zonas
    public float duracionZona = 8f;   // Cuánto tiempo permanece activa la zona

    [Header("Área de Spawn")]
    public Vector3 areaCentro = Vector3.zero;
    public Vector3 areaTamano = new Vector3(20f, 0f, 20f);

    void Start()
    {
        if (dangerousZoneObject != null)
        {
            dangerousZoneObject.SetActive(false);
        }
        StartCoroutine(RutinaSpawn());
    }

    IEnumerator RutinaSpawn()
    {
        while (true)
        {
            // Esperar un tiempo aleatorio
            float espera = Random.Range(minIntervalo, maxIntervalo);
            yield return new WaitForSeconds(espera);

            // Generar posición aleatoria dentro del área
            Vector3 posAleatoria = areaCentro + new Vector3(
                Random.Range(-areaTamano.x / 2, areaTamano.x / 2),
                0,
                Random.Range(-areaTamano.z / 2, areaTamano.z / 2)
            );

            if (dangerousZoneObject != null)
            {
                dangerousZoneObject.transform.position = posAleatoria;
                dangerousZoneObject.SetActive(true);

                // Mantener activa la zona durante cierto tiempo
                yield return new WaitForSeconds(duracionZona);

                dangerousZoneObject.SetActive(false);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(areaCentro, areaTamano);
    }
}