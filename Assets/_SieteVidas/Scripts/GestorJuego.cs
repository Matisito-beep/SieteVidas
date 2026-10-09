using System;
using UnityEngine;

public class GestorJuego : MonoBehaviour
{
    public static GestorJuego Instancia { get; private set; }

    [Header("Vidas")]
    [Tooltip("Siete vidas, como todo gato")]
    public int vidasMaximas = 7;

    public int Vidas { get; private set; }
    public int PescadosRecogidos { get; private set; }
    public int TotalPescados { get; private set; }
    public float TiempoTranscurrido { get; private set; }
    public bool JuegoTerminado { get; private set; }

    
    public event Action<int> AlCambiarVidas;              // vidas actuales
    public event Action<int, int> AlCambiarPescados;      // pescados recogidos, total
    public event Action<bool> AlTerminarJuego;            // true = victoria, false = derrota

    void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;
        Vidas = vidasMaximas;
    }

    void Start()
    {
        TotalPescados = FindObjectsByType<PescadoColeccionable>(FindObjectsSortMode.None).Length;

        AlCambiarVidas?.Invoke(Vidas);
        AlCambiarPescados?.Invoke(PescadosRecogidos, TotalPescados);
        Debug.Log($"Nivel iniciado. Vidas: {Vidas} | Pescados: 0/{TotalPescados}");
    }

    void Update()
    {
        if (!JuegoTerminado)
        {
            TiempoTranscurrido += Time.deltaTime;
        }
    }

    public void SumarPescado()
    {
        if (JuegoTerminado) return;

        PescadosRecogidos++;
        AlCambiarPescados?.Invoke(PescadosRecogidos, TotalPescados);
        Debug.Log($"Pescados: {PescadosRecogidos}/{TotalPescados}");

        // Si recogio todos los pescados, gana
        if (PescadosRecogidos >= TotalPescados)
        {
            Ganar();
        }
    }

    public void PerderVida()
    {
        if (JuegoTerminado) return;

        Vidas--;
        AlCambiarVidas?.Invoke(Vidas);
        Debug.Log($"Vidas restantes: {Vidas}");

        if (Vidas <= 0)
        {
            TerminarJuego(false);
        }
    }

    public void Ganar()
    {
        TerminarJuego(true);
    }

    private void TerminarJuego(bool gano)
    {
        if (JuegoTerminado) return;

        JuegoTerminado = true;
        AlTerminarJuego?.Invoke(gano);
        Debug.Log(gano ? "¡VICTORIA! Recogiste todos los pescados" : "DERROTA: se acabaron las 7 vidas");
    }

    void OnDestroy()
    {
        if (Instancia == this) Instancia = null;
    }
}