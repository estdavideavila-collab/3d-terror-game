
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class SonidoLoboAleatorio : MonoBehaviour
{
    public float tiempoMinimo = 20f;
    public float tiempoMaximo = 40f;

    private AudioSource audioLobo;

    void Start()
    {
        audioLobo = GetComponent<AudioSource>();
        audioLobo.playOnAwake = false;
        audioLobo.loop = false;

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
                audioLobo.Play();
                yield return new WaitWhile(() => audioLobo.isPlaying);
            }
        }
    }
}

