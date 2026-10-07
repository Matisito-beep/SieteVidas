using System.Collections;
using UnityEngine;

// Hace que un disco transparente crezca y se desvanezca sobre el charco,
// como una onda en el agua cuando cae una gota.
public class OndaCharco : MonoBehaviour
{
    [Header("Tamaño")]
    public float escalaInicial = 0.3f;
    public float escalaFinal = 1f;

    [Header("Tiempo")]
    public float duracion = 1.2f;
    [Tooltip("Espera aleatoria entre ondas, para que no salgan todas juntas")]
    public float esperaMinima = 0.5f;
    public float esperaMaxima = 2.5f;

    [Header("Transparencia")]
    [Range(0f, 1f)] public float opacidadInicial = 0.5f;

    private Renderer renderizador;
    private Color colorBase;
    private Vector3 escalaOriginal;

    void Start()
    {
        renderizador = GetComponent<Renderer>();
        colorBase = renderizador.material.color;
        escalaOriginal = transform.localScale;
        StartCoroutine(RutinaOndas());
    }

    // Corrutina: repite la onda para siempre, con pausas al azar
    private IEnumerator RutinaOndas()
    {
        while (true)
        {
            MostrarOnda(0f, 0f);
            yield return new WaitForSeconds(Random.Range(esperaMinima, esperaMaxima));

            float tiempo = 0f;
            while (tiempo < duracion)
            {
                tiempo += Time.deltaTime;
                float progreso = tiempo / duracion;
                float escala = Mathf.Lerp(escalaInicial, escalaFinal, progreso);
                float opacidad = Mathf.Lerp(opacidadInicial, 0f, progreso);
                MostrarOnda(escala, opacidad);
                yield return null;
            }
        }
    }

    private void MostrarOnda(float escala, float opacidad)
    {
        transform.localScale = new Vector3(escalaOriginal.x * escala, escalaOriginal.y, escalaOriginal.z * escala);

        Color color = colorBase;
        color.a = opacidad;
        renderizador.material.color = color;
    }
}