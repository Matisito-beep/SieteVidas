using System.Collections;
using UnityEngine;
using StarterAssets;

// Da "vida" al salto del gato: se estira al saltar, se aplasta al aterrizar
// y se inclina en el aire. Usa corrutinas para las animaciones de escala.
[RequireComponent(typeof(ThirdPersonController))]
public class EfectoSaltoGato : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("El contenedor visual del gato (CatVisual)")]
    public Transform visual;

    [Header("Estirar al saltar")]
    public Vector3 escalaEstirado = new Vector3(0.85f, 1.25f, 0.85f);
    public float duracionEstirado = 0.12f;

    [Header("Aplastar al aterrizar")]
    public Vector3 escalaAplastado = new Vector3(1.2f, 0.75f, 1.2f);
    public float duracionAplastado = 0.1f;

    [Header("Inclinacion en el aire")]
    [Tooltip("Grados maximos que el gato inclina la nariz")]
    public float inclinacionMaxima = 25f;
    [Tooltip("Que tan rapido se inclina")]
    public float velocidadInclinacion = 8f;

    private ThirdPersonController controlador;
    private CharacterController controladorPersonaje;
    private Coroutine rutinaEscala;
    private Vector3 escalaBase;
    private bool estabaEnSuelo = true;
    private float inclinacionActual;

    void Start()
    {
        controlador = GetComponent<ThirdPersonController>();
        controladorPersonaje = GetComponent<CharacterController>();
        escalaBase = visual.localScale;
    }

    void Update()
    {
        bool enSuelo = controlador.Grounded;
        float velocidadVertical = controladorPersonaje.velocity.y;

        // Acaba de saltar (estaba en el suelo, ahora va subiendo)
        if (estabaEnSuelo && !enSuelo && velocidadVertical > 0.1f)
        {
            ReproducirEfectoEscala(escalaEstirado, duracionEstirado);
        }
        // Acaba de aterrizar
        else if (!estabaEnSuelo && enSuelo)
        {
            ReproducirEfectoEscala(escalaAplastado, duracionAplastado);
        }

        estabaEnSuelo = enSuelo;

        // Inclinacion: nariz arriba al subir, nariz abajo al caer
        float inclinacionObjetivo = enSuelo ? 0f : Mathf.Clamp(-velocidadVertical * 4f, -inclinacionMaxima, inclinacionMaxima);
        inclinacionActual = Mathf.Lerp(inclinacionActual, inclinacionObjetivo, Time.deltaTime * velocidadInclinacion);
        visual.localRotation = Quaternion.Euler(inclinacionActual, 0f, 0f);
    }

    // Detiene el efecto anterior (si habia uno) y parte el nuevo
    private void ReproducirEfectoEscala(Vector3 escalaObjetivo, float duracion)
    {
        if (rutinaEscala != null)
        {
            StopCoroutine(rutinaEscala);
        }
        rutinaEscala = StartCoroutine(RutinaDeformar(escalaObjetivo, duracion));
    }

    // Corrutina: va a la escala deformada y luego vuelve a la normal
    private IEnumerator RutinaDeformar(Vector3 escalaObjetivo, float duracion)
    {
        Vector3 deformada = Vector3.Scale(escalaBase, escalaObjetivo);
        yield return CambiarEscala(deformada, duracion);
        yield return CambiarEscala(escalaBase, duracion * 1.5f);
        rutinaEscala = null;
    }

    // Corrutina: cambia la escala suavemente durante 'duracion' segundos
    private IEnumerator CambiarEscala(Vector3 meta, float duracion)
    {
        Vector3 inicio = visual.localScale;
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            visual.localScale = Vector3.Lerp(inicio, meta, tiempo / duracion);
            yield return null; // espera al siguiente frame
        }

        visual.localScale = meta;
    }
}