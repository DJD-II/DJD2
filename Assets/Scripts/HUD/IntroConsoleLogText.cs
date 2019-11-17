using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IntroConsoleLogText : MonoBehaviour
{
    [SerializeField]
    private Text consoleText = null;
    [SerializeField]
    private string[] consoleTexts = new string[0];
    [SerializeField]
    private AnimationCurve glitchProgress = null;
    [Header("V Ram")]
    [SerializeField]
    private AnimationCurve vramIntensity = null;
    [SerializeField]
    private AnimationCurve vramMultiplier = null;
    [SerializeField]
    private float vramSpeed = 2f;
    [SerializeField]
    private ShaderEffect_CorruptedVram vRamController = null;
    [Header("Color Bleed")]
    [SerializeField]
    private AnimationCurve bleedIntensity = null;
    [SerializeField]
    private AnimationCurve bleedMultiplier = null;
    [SerializeField]
    private float bleedSpeed = 2f;
    [SerializeField]
    private ShaderEffect_BleedingColors bleedController = null;
    [Header("CRT")]
    [SerializeField]
    private ShaderEffect_CRT crtController = null;
    [Header("Scanner")]
    [SerializeField]
    private AnimationCurve scannerIntensity = null;
    [SerializeField]
    private float scannerMultiplier = 10f;
    [SerializeField]
    private float scannerSpeed = 2f;
    [SerializeField]
    private ShaderEffect_Scanner scannerController = null;
    [Header("SFX")]
    [SerializeField]
    private AudioSource SFX = null;
    private float timer = 0;
    private bool active = false;

    public void StartGlitch()
    {
        scannerController.enabled = true;
        vRamController.enabled = true;
        bleedController.enabled = true;
        crtController.enabled = true;
        SFX.enabled = true;

        timer = 0;
        active = true;
        SFX.Play();
    }

    public void StartConsoleTexts()
    {
        consoleText.text = "";
        StartCoroutine(ShowLogText());
    }

    private IEnumerator ShowLogText()
    {
        foreach (string s in consoleTexts)
        {
            consoleText.text += "\n" + s;
            yield return new WaitForSecondsRealtime(0.3f);
        }
    }

    public void StopGlitch()
    {
        active = false;
        SFX.enabled = false;
        scannerController.enabled = false;
        vRamController.enabled = false;
        bleedController.enabled = false;
        crtController.enabled = false;
    }

    private void Update()
    {
        if (!active)
            return;

        timer += Time.unscaledDeltaTime;

        float m = glitchProgress.Evaluate(timer);
        float ramInt = vramIntensity.Evaluate(Time.unscaledTime * vramSpeed * m) * vramMultiplier.Evaluate(Time.unscaledTime * m);
        SFX.volume = ramInt;
        vRamController.enabled = crtController.enabled = ramInt > 0.5f;
        vRamController.Shift = ramInt * m;
        //SFX.pitch = ramInt / vramMultiplier;
        bleedController.Intensity = bleedIntensity.Evaluate(Time.unscaledTime * bleedSpeed * m) * bleedMultiplier.Evaluate(Time.unscaledTime * m) * m;
        float scannerInt = scannerIntensity.Evaluate(Time.unscaledTime * scannerSpeed * m) * scannerMultiplier;
        scannerController.enabled = scannerInt > 0.5f;
        scannerController.Area = scannerInt * m;
    }
}
