using System.Collections;
using StarterAssets;
using TMPro;
using UnityEngine;

public class PantallaResumen : MonoBehaviour
{
    [Header("Panel")]
    public CanvasGroup panelResumen;
    public TMP_Text textoTitulo;
    public TMP_Text textoDetalle;

    public float esperaAntesDeMostrar = 0.8f;
    public float duracionFade = 0.6f;

    public Color colorVictoria = new Color(1f, 0.85f, 0.2f);
    public Color colorDerrota = new Color(1f, 0.3f, 0.3f);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OcultarPanel();
        GestorJuego.Instancia.AlTerminarJuego += MostrarResumen;
        
    }


    void OnDestroy()
    {
        if (GestorJuego.Instancia != null)
        {
            GestorJuego.Instancia.AlTerminarJuego -= MostrarResumen;
        }
    }

    private void OcultarPanel()
    {
        panelResumen.alpha = 0f;
        panelResumen.interactable = false;
        panelResumen.blocksRaycasts = false;
    }

    private void MostrarResumen(bool gano)
    {
        DetenerGato();
        LiberarCursor();

        GestorJuego gestor = GestorJuego.Instancia;

        textoTitulo.text = gano ? "¡Ganaste!" : "Perdiste";
        textoTitulo.color = gano ? colorVictoria : colorDerrota;

        int minutos = Mathf.FloorToInt(gestor.TiempoTranscurrido / 60f);
        int segundos = Mathf.FloorToInt(gestor.TiempoTranscurrido % 60f);

        textoDetalle.text =
            $"Pescados: {gestor.PescadosRecogidos}/{gestor.TotalPescados}\n" +
            $"Vidas restantes: {gestor.Vidas}\n" +
            $"Tiempo: {minutos:00}:{segundos:00}";

        StartCoroutine(RutinaMostrarPanel());
    }

    private void DetenerGato()
    {
        GameObject gato = GameObject.FindWithTag("Player");
        if (gato == null) return;

        if (gato.TryGetComponent(out ThirdPersonController controlador))
        {
            controlador.enabled = false;
        }

        if (gato.TryGetComponent(out StarterAssetsInputs entradas))
        {
            entradas.move = Vector2.zero;
            entradas.look = Vector2.zero;
            entradas.jump = false;
            entradas.sprint = false;
            entradas.cursorLocked = false;       
            entradas.cursorInputForLook = false;
        }

        Animator animador = gato.GetComponentInChildren<Animator>();
        if (animador != null)
        {
            animador.SetFloat("Speed", 0f);
            animador.SetBool("Jump", false);
            animador.SetBool("FreeFall", false);
            animador.SetBool("Grounded", true);
        }
    }

     private void LiberarCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private IEnumerator RutinaMostrarPanel()
    {
        yield return new WaitForSeconds(esperaAntesDeMostrar);

        float tiempo = 0f;
        while (tiempo < duracionFade)
        {
            tiempo += Time.deltaTime;
            panelResumen.alpha = Mathf.Clamp01(tiempo / duracionFade);
            yield return null;
        }

        panelResumen.alpha = 1f;
        panelResumen.interactable = true;
        panelResumen.blocksRaycasts = true;
    }

    public void VolverAlMenu()
    {
        Debug.Log("Volver al menu: pendiente de la carga asincronica;(");
    }
}
