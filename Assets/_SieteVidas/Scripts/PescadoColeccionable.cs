using System.Collections;
using UnityEngine;

// Pescado coleccionable: gira, flota y al recogerlo
[RequireComponent(typeof(Collider))]
public class PescadoColeccionable : MonoBehaviour
{
    [Header("Movimiento en reposo")]
    public float velocidadRotacion = 90f;
    public float alturaFlotacion = 0.15f;
    public float velocidadFlotacion = 2f;

    [Header("Al recogerlo")]
    public float duracionRecoleccion = 0.25f;

    private Vector3 posicionInicial;
    private bool recogido;

    void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void Start()
    {
        posicionInicial = transform.position;
    }

    void Update()
    {
        if (recogido) return;

        transform.Rotate(Vector3.up, velocidadRotacion * Time.deltaTime, Space.World);
        float desplazamiento = Mathf.Sin(Time.time * velocidadFlotacion) * alturaFlotacion;
        transform.position = posicionInicial + Vector3.up * desplazamiento;
    }

    void OnTriggerEnter(Collider otro)
    {
        if (recogido || !otro.CompareTag("Player")) return;

        recogido = true;
        GestorJuego.Instancia.SumarPescado();
        StartCoroutine(RutinaRecoger());
    }

    // Corrutina: se encoge y sube antes de desaparecer
    private IEnumerator RutinaRecoger()
    {
        Vector3 escalaInicial = transform.localScale;
        Vector3 desde = transform.position;
        float tiempo = 0f;

        while (tiempo < duracionRecoleccion)
        {
            tiempo += Time.deltaTime;
            float progreso = tiempo / duracionRecoleccion;
            transform.localScale = Vector3.Lerp(escalaInicial, Vector3.zero, progreso);
            transform.position = desde + Vector3.up * (progreso * 0.5f);
            yield return null;
        }

        gameObject.SetActive(false);
    }
}