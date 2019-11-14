using UnityEngine;

[ExecuteInEditMode]
sealed public class ShaderEffect_CRT : MonoBehaviour
{
    [SerializeField]
    private float scanlineIntensity = 100;
    [SerializeField]
    private int scanlineWidth = 1;
    private Material material_Displacement;
    private Material material_Scanlines;
    private RenderTexture intermideateRT;

    public float ScanlineIntensity { get { return scanlineIntensity; } set { scanlineIntensity = value; } }
    public int ScanlineWidth { get { return scanlineWidth; } set { scanlineWidth = value; } }

    private void Awake()
    {
        material_Scanlines = new Material(Shader.Find("Hidden/Scanlines"));
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        intermideateRT = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);

        material_Scanlines.SetFloat("_Intensity", scanlineIntensity * 0.01f);
        material_Scanlines.SetFloat("_ValueX", scanlineWidth);

        Graphics.Blit(source, intermideateRT, material_Scanlines);
        Graphics.Blit(intermideateRT, destination);

        RenderTexture.ReleaseTemporary(intermideateRT);
    }
}
