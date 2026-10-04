using System.Collections;
using StarterAssets;
using UnityEngine;

public class GestorAudio : MonoBehaviour
{
    [Header("Musica")]
    public AudioClip musicaNivel;
    public float volumenMusica = 0.4f;
    public float duracionFadeMusica = 1f;

    public AudioClip sonidoPescado;
    public AudioClip sonidoGolpe;
    public AudioClip sonidoSalto;
    public AudioClip sonidoVictoria;
    public AudioClip sonidoDerrota;
    public float volumenEfectos = 0.8f;

    private AudioSource fuenteMusica;
    private AudioSource fuenteEfectos;
    private int ultimasVidas;
    private int ultimosPescados;
    private ThirdPersonController controlador;
    private CharacterController controladorPersonaje;
    private bool estabaEnSuelo = true;

    void Awake()
    {
        
        fuenteMusica = gameObject.AddComponent<AudioSource>();
        fuenteMusica.loop = true;
        fuenteMusica.playOnAwake = false;
        fuenteMusica.volume = volumenMusica;

        fuenteEfectos = gameObject.AddComponent<AudioSource>();
        fuenteEfectos.playOnAwake = false;
        fuenteEfectos.volume = volumenEfectos;
    }

    void Start()
    {
        GestorJuego gestor = GestorJuego.Instancia;

      
        ultimasVidas = gestor.Vidas;
        ultimosPescados = gestor.PescadosRecogidos;

        gestor.AlCambiarVidas += ReaccionarVidas;
        gestor.AlCambiarPescados += ReaccionarPescados;
        gestor.AlTerminarJuego += ReaccionarFin;

        GameObject gato = GameObject.FindWithTag("Player");
        if (gato != null)
        {
            controlador = gato.GetComponent<ThirdPersonController>();
            controladorPersonaje = gato.GetComponent<CharacterController>();
        }

        if (musicaNivel != null)
        {
            fuenteMusica.clip = musicaNivel;
            fuenteMusica.Play();
        }
    }

    void OnDestroy()
    {
        if (GestorJuego.Instancia != null)
        {
            GestorJuego.Instancia.AlCambiarVidas -= ReaccionarVidas;
            GestorJuego.Instancia.AlCambiarPescados -= ReaccionarPescados;
            GestorJuego.Instancia.AlTerminarJuego -= ReaccionarFin;
        }
    }

    void Update()
    {
        
        if (controlador == null || !controlador.enabled) return;

        bool enSuelo = controlador.Grounded;
        if (estabaEnSuelo && !enSuelo && controladorPersonaje.velocity.y > 0.1f)
        {
            Reproducir(sonidoSalto);
        }
        estabaEnSuelo = enSuelo;
    }

    private void ReaccionarVidas(int vidas)
    {
        if (vidas < ultimasVidas) Reproducir(sonidoGolpe);
        ultimasVidas = vidas;
    }

    private void ReaccionarPescados(int recogidos, int total)
    {
        if (recogidos > ultimosPescados) Reproducir(sonidoPescado);
        ultimosPescados = recogidos;
    }

    private void ReaccionarFin(bool gano)
    {
        StartCoroutine(RutinaApagarMusica());
        Reproducir(gano ? sonidoVictoria : sonidoDerrota);
    }

    private void Reproducir(AudioClip clip)
    {
        if (clip != null) fuenteEfectos.PlayOneShot(clip);
    }

    
    private IEnumerator RutinaApagarMusica()
    {
        float volumenInicial = fuenteMusica.volume;
        float tiempo = 0f;

        while (tiempo < duracionFadeMusica)
        {
            tiempo += Time.deltaTime;
            fuenteMusica.volume = Mathf.Lerp(volumenInicial, 0f, tiempo / duracionFadeMusica);
            yield return null;
        }

        fuenteMusica.Stop();
    }
}