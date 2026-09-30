using UnityEngine;

// Zona invisible debajo del nivel: si el gato cae, pierde una vida y reaparece.
[RequireComponent(typeof(Collider))]
public class ZonaCaida : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider otro)
    {
        if (otro.TryGetComponent(out SaludJugador salud))
        {
            salud.CaerYReaparecer();
        }
    }
}