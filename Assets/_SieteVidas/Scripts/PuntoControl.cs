using UnityEngine;

// Al pasar por aqui, el gato reaparecera en este punto si cae.
[RequireComponent(typeof(Collider))]
public class PuntoControl : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider otro)
    {
        if (otro.TryGetComponent(out SaludJugador salud))
        {
            salud.FijarPuntoControl(transform.position);
            Debug.Log($"Punto de control alcanzado: {name}");
        }
    }
}