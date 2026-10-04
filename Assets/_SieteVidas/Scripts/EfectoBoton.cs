using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class EfectoBoton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
   
    public float escalaEncima = 1.1f;
    public float duracion = 0.1f;

    private Vector3 escalaBase;
    private Coroutine rutina;

    void Awake()
    {
        escalaBase = transform.localScale;
    }

    void OnDisable()
    {
    
        transform.localScale = escalaBase;
    }

    public void OnPointerEnter(PointerEventData datos)
    {
        Escalar(escalaBase * escalaEncima);
    }

    public void OnPointerExit(PointerEventData datos)
    {
        Escalar(escalaBase);
    }

    private void Escalar(Vector3 meta)
    {
        if (rutina != null) StopCoroutine(rutina);
        rutina = StartCoroutine(RutinaEscalar(meta));
    }


    private IEnumerator RutinaEscalar(Vector3 meta)
    {
        Vector3 inicio = transform.localScale;
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.unscaledDeltaTime;
            transform.localScale = Vector3.Lerp(inicio, meta, tiempo / duracion);
            yield return null;
        }

        transform.localScale = meta;
    }
}