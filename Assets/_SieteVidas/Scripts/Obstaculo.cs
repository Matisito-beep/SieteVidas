using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Obstaculo : MonoBehaviour
{
    [Header("Efecto al golpear (opcional)")]
    [Tooltip("Prefab de particulas que aparece donde esta el gato al recibir el golpe")]
    public GameObject efectoGolpe;
    [Tooltip("Segundos antes de borrar el efecto")]
    public float duracionEfecto = 2f;

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
        if (!otro.TryGetComponent(out SaludJugador salud)) return;

        
        bool habraGolpe = !salud.EsInvulnerable && !GestorJuego.Instancia.JuegoTerminado;

        salud.RecibirGolpe();

        if (habraGolpe)
        {
            MostrarEfecto(otro.transform.position);
        }
    }

    private void MostrarEfecto(Vector3 posicion)
    {
        if (efectoGolpe == null) return;

        
        GameObject efecto = Instantiate(efectoGolpe, posicion, efectoGolpe.transform.rotation);
        Destroy(efecto, duracionEfecto);
    }
}