using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneSpawner : MonoBehaviour
{
    [Header("Prefab de la Zona")]
    public GameObject dangerousZonePrefab; // Prefab de la zona peligrosa (Invisible con Trigger)

    [Header("Control de Frecuencia y Cantidad")]
    public int maxZonasSimultaneas = 5;    // Número máximo de zonas activas a la vez
    public float minIntervalo = 3f;         // Aparecerán mucho más seguido (cada 3-6 seg)
    public float maxIntervalo = 6f;
    public float duracionZona = 10f;        // Tiempo que permanece activa cada zona

    [Header("Área de Spawn del Mapa")]
    public Vector3 areaCentro = Vector3.zero;
    public Vector3 areaTamano = new Vector3(50f, 0f, 50f); // Tamaño del mapa completo

    private List<GameObject> zonasActivas = new List<GameObject>();

    void Start()
    {
        StartCoroutine(RutinaGenerarZonas());
    }

    IEnumerator RutinaGenerarZonas()
    {
        while (true)
        {
            // Limpiar de la lista las zonas que ya fueron destruidas/desactivadas
            zonasActivas.RemoveAll(zona => zona == null || !zona.activeSelf);

            // Si aún no alcanzamos el límite máximo de zonas activas, creamos una nueva
            if (zonasActivas.Count < maxZonasSimultaneas && dangerousZonePrefab != null)
            {
                CrearNuevaZona();
            }

            // Esperar un tiempo corto para la siguiente comprobación/generación
            float espera = Random.Range(minIntervalo, maxIntervalo);
            yield return new WaitForSeconds(espera);
        }
    }

    void CrearNuevaZona()
    {
        // Calcular una posición aleatoria dentro del área del mapa
        Vector3 posAleatoria = areaCentro + new Vector3(
            Random.Range(-areaTamano.x / 2, areaTamano.x / 2),
            0f,
            Random.Range(-areaTamano.z / 2, areaTamano.z / 2)
        );

        // Instanciar o activar una zona en esa posición
        GameObject nuevaZona = Instantiate(dangerousZonePrefab, posAleatoria, Quaternion.identity);
        zonasActivas.Add(nuevaZona);

        // Programar su destrucción automática tras cumplirse la duración
        Destroy(nuevaZona, duracionZona);
    }

    private void OnDrawGizmosSelected()
    {
        // Dibuja el área de spawn en la vista de Escena en Unity
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(areaCentro, areaTamano);
    }
}