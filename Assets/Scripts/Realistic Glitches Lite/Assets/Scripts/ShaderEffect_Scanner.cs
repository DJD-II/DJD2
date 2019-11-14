using UnityEngine;

[ExecuteInEditMode]
sealed public class ShaderEffect_Scanner : MonoBehaviour
{

    [SerializeField]
    private float area = 1;
    private Material material_a, material_b;
    private RenderTexture intermideateRT;

    public float Area { get { return area; } set { area = value; } }

    // Creates a private material used to the effect
    private void Awake()
    {
        material_a = new Material(Shader.Find("Hidden/Shift"));
        material_b = new Material(Shader.Find("Hidden/Shift"));
    }

    // Postprocess the image
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        intermideateRT = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);
        material_a.SetFloat("_ValueY", area);
        material_b.SetFloat("_ValueY", -area);

        Graphics.Blit(source, intermideateRT, material_a);
        Graphics.Blit(intermideateRT, destination, material_b);

        RenderTexture.ReleaseTemporary(intermideateRT);
    }
}
