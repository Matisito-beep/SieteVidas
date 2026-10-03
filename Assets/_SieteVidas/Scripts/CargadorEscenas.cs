using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class CargadorEscenas : MonoBehaviour
{
    [Header("Panel de carga")]
    public CanvasGroup panelCarga;
    public Slider barraProgreso;
    public TMP_Text textoProgreso;

    public float tiempoMinimoCarga = 1f;
    public bool liberarCursorAlIniciar = false;

    private bool cargando;

    void Start()
    {
        MostrarPanel(false);

        if (liberarCursorAlIniciar)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }


    public void CargarEscena(string nombreEscena)
    {
        if (cargando) return;
        StartCoroutine(RutinaCargar(nombreEscena));
    }


    public void Salir()
    {
        Debug.Log("Saliendo del juego (en el editor no se cierra)");
        Application.Quit();
    }

   
    private IEnumerator RutinaCargar(string nombreEscena)
    {
        cargando = true;
        MostrarPanel(true);
        ActualizarBarra(0f);

        AsyncOperation operacion = SceneManager.LoadSceneAsync(nombreEscena);
        if (operacion == null)
        {
            Debug.LogError($"No se pudo cargar '{nombreEscena}'. Revisa que este en Build Profiles → Scene List");
            MostrarPanel(false);
            cargando = false;
            yield break;
        }

        operacion.allowSceneActivation = false;

        float tiempo = 0f;

        while (operacion.progress < 0.9f || tiempo < tiempoMinimoCarga)
        {
            tiempo += Time.unscaledDeltaTime;
            float progresoReal = Mathf.Clamp01(operacion.progress / 0.9f);
            float progresoTiempo = Mathf.Clamp01(tiempo / tiempoMinimoCarga);
            ActualizarBarra(Mathf.Min(progresoReal, progresoTiempo));
            yield return null;
        }

        ActualizarBarra(1f);
        yield return null; 

        operacion.allowSceneActivation = true;
    }

    private void MostrarPanel(bool visible)
    {
        panelCarga.alpha = visible ? 1f : 0f;
        panelCarga.blocksRaycasts = visible; 
        panelCarga.interactable = visible;
    }

    private void ActualizarBarra(float progreso)
    {
        barraProgreso.value = progreso;
        textoProgreso.text = $"Cargando... {Mathf.RoundToInt(progreso * 100)}%";
    }
}