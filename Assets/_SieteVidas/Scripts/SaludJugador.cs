using System.Collections;
using UnityEngine;

// Maneja los golpes que recibe el gato, la invulnerabilidad temporal
// (con parpadeo) y la reaparicion al caer.
[RequireComponent(typeof(CharacterController))]
public class SaludJugador : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("El contenedor visual del gato (CatVisual)")]
    public Transform visual;

    [Header("Invulnerabilidad")]
    [Tooltip("Segundos sin recibir golpes despues de uno")]
    public float tiempoInvulnerabilidad = 1.5f;
    [Tooltip("Cada cuantos segundos parpadea")]
    public float intervaloParpadeo = 0.1f;

    public bool EsInvulnerable { get; private set; }

    private CharacterController controladorPersonaje;
    private Renderer[] renderizadores;
    private Vector3 puntoReaparicion;

    void Start()
    {
        controladorPersonaje = GetComponent<CharacterController>();
        renderizadores = visual.GetComponentsInChildren<Renderer>();
        puntoReaparicion = transform.position; // el punto de inicio es el primer punto de control
    }

    // Lo llaman los obstaculos
    public void RecibirGolpe()
    {
        if (EsInvulnerable || GestorJuego.Instancia.JuegoTerminado) return;

        GestorJuego.Instancia.PerderVida();

        if (!GestorJuego.Instancia.JuegoTerminado)
        {
            StartCoroutine(RutinaInvulnerabilidad());
        }
    }

    // Lo llama la zona de caida: siempre quita vida y devuelve al punto de control
    public void CaerYReaparecer()
    {
        if (GestorJuego.Instancia.JuegoTerminado) return;

        GestorJuego.Instancia.PerderVida();
        Teletransportar(puntoReaparicion);

        if (!GestorJuego.Instancia.JuegoTerminado && !EsInvulnerable)
        {
            StartCoroutine(RutinaInvulnerabilidad());
        }
    }

    // Lo llaman los puntos de control
    public void FijarPuntoControl(Vector3 posicion)
    {
        puntoReaparicion = posicion;
    }

    private void Teletransportar(Vector3 posicion)
    {
        // El CharacterController no deja mover el transform directamente,
        // asi que se apaga un instante
        controladorPersonaje.enabled = false;
        transform.position = posicion;
        controladorPersonaje.enabled = true;
    }

    // Corrutina: el gato parpadea y no recibe golpes por unos segundos
    private IEnumerator RutinaInvulnerabilidad()
    {
        EsInvulnerable = true;

        float tiempo = 0f;
        bool visible = true;

        while (tiempo < tiempoInvulnerabilidad)
        {
            visible = !visible;
            MostrarGato(visible);
            yield return new WaitForSeconds(intervaloParpadeo);
            tiempo += intervaloParpadeo;
        }

        MostrarGato(true);
        EsInvulnerable = false;
    }

    private void MostrarGato(bool visible)
    {
        foreach (Renderer renderizador in renderizadores)
        {
            renderizador.enabled = visible;
        }
    }
}