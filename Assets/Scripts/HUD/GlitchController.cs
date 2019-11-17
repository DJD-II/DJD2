using System.Collections;
using UnityEngine;

sealed public class GlitchController : MonoBehaviour
{
    [SerializeField] private AnimationCurve glitchProgress = null;
    [SerializeField] private PlayerController controller = null;
    [Header("V Ram")]
    [SerializeField] private AnimationCurve vramIntensity = null;
    [SerializeField] private AnimationCurve vramMultiplier = null;
    [SerializeField] private float vramSpeed = 2f;
    [SerializeField] private ShaderEffect_CorruptedVram vRamController = null;
    [Header("Color Bleed")]
    [SerializeField] private AnimationCurve bleedIntensity = null;
    [SerializeField] private AnimationCurve bleedMultiplier = null;
    [SerializeField] private float bleedSpeed = 2f;
    [SerializeField] private ShaderEffect_BleedingColors bleedController = null;
    [Header("CRT")]
    [SerializeField] private ShaderEffect_CRT crtController = null;
    [Header("Scanner")]
    [SerializeField] private AnimationCurve scannerIntensity = null;
    [SerializeField] private float scannerMultiplier = 10f;
    [SerializeField] private float scannerSpeed = 2f;
    [SerializeField] private ShaderEffect_Scanner scannerController = null;
    [Header("SFX")]
    [SerializeField] private AudioSource SFX = null;
    private Coroutine impulseCoroutine = null;

    private void Update()
    {
        if (impulseCoroutine != null)
            return;

        if (controller.Hp.Scalar <= 0.3f)
            ApplyGlitch(1f - controller.Hp.Scalar);
        else
            EndGlitch();
    }

    private void EndGlitch()
    {
        vRamController.enabled = crtController.enabled = scannerController.enabled = false;
        SFX.volume = 0f;
    }

    private void ApplyGlitch(float intensity)
    {
        float m = glitchProgress.Evaluate(intensity);
        float ramInt = vramIntensity.Evaluate(Time.unscaledTime * vramSpeed * m) * vramMultiplier.Evaluate(Time.unscaledTime * m);
        SFX.volume = ramInt;
        vRamController.enabled = crtController.enabled = ramInt > 0.5f;
        vRamController.Shift = ramInt * m;
        bleedController.Intensity = bleedIntensity.Evaluate(Time.unscaledTime * bleedSpeed * m) * bleedMultiplier.Evaluate(Time.unscaledTime * m) * m;
        float scannerInt = scannerIntensity.Evaluate(Time.unscaledTime * scannerSpeed * m) * scannerMultiplier;
        scannerController.enabled = scannerInt > 0.5f;
        scannerController.Area = scannerInt * m;
    }

    public void Impulse(float intensity, float time)
    {
        if (impulseCoroutine != null)
        {
            StopCoroutine(impulseCoroutine);
            impulseCoroutine = null;
        }

        impulseCoroutine = StartCoroutine(ApplyImpulse(intensity, time));
    }

    private IEnumerator ApplyImpulse(float intensity, float time)
    {
        ApplyGlitch(intensity);

        yield return new WaitForSecondsRealtime(time);

        EndGlitch();

        impulseCoroutine = null;
    }
}
