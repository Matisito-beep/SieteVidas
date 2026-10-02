using System.Collections;
using TMPro;
using UnityEngine;
public class HUDJuego : MonoBehaviour
{
    [Header("Textos")]
    public TMP_Text textoVidas;
    public TMP_Text textoPescados;
    public TMP_Text textoTiempo;

    [Header("Efecto al cambiar")]
    public float escalaSalto = 1.3f;
    public float duracionSalto = 0.2f;
    public Color colorGolpe = new Color(1f, 0.3f, 0.3f);

    private Color colorOriginalVidas;
    private Coroutine rutinaVidas;
    private Coroutine rutinaPescados;
    private bool primeraActualizacion = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOriginalVidas = textoVidas.color;

        GestorJuego gestor = GestorJuego.Instancia;
        gestor.AlCambiarVidas += ActualizarVidas;
        gestor.AlCambiarPescados += ActualizarPescados;

        textoVidas.text = $"Vidas: {gestor.Vidas}";
        textoPescados.text = $"Pescados: {gestor.PescadosRecogidos}/{gestor.TotalPescados}";
        primeraActualizacion = false;
    }

    void OnDestroy()
    {
        if (GestorJuego.Instancia != null)
        {
            GestorJuego.Instancia.AlCambiarVidas -= ActualizarVidas;
            GestorJuego.Instancia.AlCambiarPescados -= ActualizarPescados;
        }
    }
    // Update is called once per frame
    void Update()
    {
        float tiempo = GestorJuego.Instancia.TiempoTranscurrido;
        int minutos = Mathf.FloorToInt(tiempo / 60f);
        int segundos = Mathf.FloorToInt(tiempo % 60f);
        textoTiempo.text = $"Tiempo: {minutos:00}:{segundos:00}";
    }

    private void ActualizarVidas(int vidas)
    {
        textoVidas.text = $"Vidas: {vidas}";
        if (primeraActualizacion) return;

        if (rutinaVidas != null) StopCoroutine(rutinaVidas);
        rutinaVidas = StartCoroutine(RutinaSalto(textoVidas, true));
    }

    private void ActualizarPescados(int recogidos, int total)
    {
        textoPescados.text = $"Pescados: {recogidos}/{total}";
        if (primeraActualizacion) return;

        if (rutinaPescados != null) StopCoroutine(rutinaPescados);
        rutinaPescados = StartCoroutine(RutinaSalto(textoPescados, false));
    }

    private IEnumerator RutinaSalto(TMP_Text texto, bool esGolpe)
    {
        Transform t = texto.transform;
        Vector3 escalaNormal = Vector3.one;
        Vector3 escalaGrande = Vector3.one * escalaSalto;

        if (esGolpe) texto.color = colorGolpe;

        float tiempo = 0f;
        while (tiempo < duracionSalto)
        {
            tiempo += Time.deltaTime;
            float progreso = tiempo / duracionSalto;
            // Sube hasta la mitad del tiempo y baja en la otra mitad
            float curva = Mathf.Sin(progreso * Mathf.PI);
            t.localScale = Vector3.Lerp(escalaNormal, escalaGrande, curva);
            yield return null;
        }

        t.localScale = escalaNormal;

        if (esGolpe)
        {
            yield return new WaitForSeconds(0.3f);
            texto.color = colorOriginalVidas;
        }
    }
}
