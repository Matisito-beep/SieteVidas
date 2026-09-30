
using System.Collections;
using UnityEngine;
using StarterAssets;

// Configuracion del salto del gato, se estira al saltar, se aplasta al aterrizar
// y se inclina en el aire. Usa corrutinas para las animaciones de escala.
[RequireComponent(typeof(ThirdPersonController))]
public class CatJumpEffect : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("El objeto contenedor del gato (CatVisual)")]
    public Transform visual;

    [Header("Estirar al saltar")]
    public Vector3 stretchScale = new Vector3(0.85f, 1.25f, 0.85f);
    public float stretchDuration = 0.12f;

    [Header("Aplastar al aterrizar")]
    public Vector3 squashScale = new Vector3(1.2f, 0.75f, 1.2f);
    public float squashDuration = 0.1f;

    [Header("Inclinacion en el aire")]
    [Tooltip("Grados maximos que el gato inclina la nariz")]
    public float maxTilt = 35f;
    [Tooltip("Que tan rapido se inclina")]
    public float tiltSpeed = 8f;

    private ThirdPersonController controller;
    private CharacterController characterController;
    private Coroutine scaleRoutine;
    private Vector3 baseScale;
    private bool wasGrounded = true;
    private float currentTilt;

    void Start()
    {
        controller = GetComponent<ThirdPersonController>();
        characterController = GetComponent<CharacterController>();
        baseScale = visual.localScale;
    }

    void Update()
    {
        bool grounded = controller.Grounded;
        float verticalSpeed = characterController.velocity.y;

        // Acaba de saltar (estaba en el suelo, ahora va subiendo)
        if (wasGrounded && !grounded && verticalSpeed > 0.1f)
        {
            PlayScaleEffect(stretchScale, stretchDuration);
        }
        // Acaba de aterrizar
        else if (!wasGrounded && grounded)
        {
            PlayScaleEffect(squashScale, squashDuration);
        }

        wasGrounded = grounded;

        // Inclinacion: nariz arriba al subir, nariz abajo al caer
        float targetTilt = grounded ? 0f : Mathf.Clamp(-verticalSpeed * 4f, -maxTilt, maxTilt);
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * tiltSpeed);
        visual.localRotation = Quaternion.Euler(currentTilt, 0f, 0f);
    }

    // Detiene el efecto anterior (si habia uno) y parte el nuevo
    private void PlayScaleEffect(Vector3 targetScale, float duration)
    {
        if (scaleRoutine != null)
        {
            StopCoroutine(scaleRoutine);
        }
        scaleRoutine = StartCoroutine(ScalePunch(targetScale, duration));
    }

    // Corrutina: va a la escala deformada y luego vuelve a la normal
    private IEnumerator ScalePunch(Vector3 targetScale, float duration)
    {
        Vector3 deformed = Vector3.Scale(baseScale, targetScale);
        yield return ScaleTo(deformed, duration);
        yield return ScaleTo(baseScale, duration * 1.5f);
        scaleRoutine = null;
    }

    // Corrutina: cambia la escala suavemente durante 'duration' segundos
    private IEnumerator ScaleTo(Vector3 goal, float duration)
    {
        Vector3 start = visual.localScale;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            visual.localScale = Vector3.Lerp(start, goal, time / duration);
            yield return null; // espera al siguiente frame
        }

        visual.localScale = goal;
    }
}