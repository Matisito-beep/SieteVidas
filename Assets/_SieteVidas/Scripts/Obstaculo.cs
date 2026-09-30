using UnityEngine;

// Obstaculo que quita una vida al tocarlo
[RequireComponent(typeof(Collider))]
public class Obstaculo : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider otro)
    {
        IntentarGolpear(otro);
    }

    void OnTriggerStay(Collider otro)
    {
        IntentarGolpear(otro);
    }

    private void IntentarGolpear(Collider otro)
    {
        if (!otro.CompareTag("Player")) return;

        if (otro.TryGetComponent(out SaludJugador salud))
        {
            salud.RecibirGolpe();
        }
    }
}