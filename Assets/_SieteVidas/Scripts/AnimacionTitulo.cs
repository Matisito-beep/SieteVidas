using UnityEngine;

public class AnimacionTitulo : MonoBehaviour
{
    [Header("Flotar")]
    public float alturaFlotacion = 10f;
    public float velocidadFlotacion = 1.5f;

    public float anguloBalanceo = 3f;
    public float velocidadBalanceo = 1f;

    private RectTransform rect;
    private Vector2 posicionInicial;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        posicionInicial = rect.anchoredPosition;
    }

    void Update()
    {
        float desplazamiento = Mathf.Sin(Time.time * velocidadFlotacion) * alturaFlotacion;
        rect.anchoredPosition = posicionInicial + Vector2.up * desplazamiento;

        float angulo = Mathf.Sin(Time.time * velocidadBalanceo) * anguloBalanceo;
        rect.localRotation = Quaternion.Euler(0f, 0f, angulo);
    }
}