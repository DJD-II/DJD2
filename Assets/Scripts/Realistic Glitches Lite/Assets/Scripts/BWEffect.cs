using UnityEngine;

[ExecuteInEditMode]
sealed public class BWEffect : MonoBehaviour
{

    [SerializeField]
    private float intensity = 1;
    private Material material;

    public float Intensity { get { return intensity; } set { intensity = value; } }

    // Creates a private material used to the effect
    private void Awake()
    {
        material = new Material(Shader.Find("Hidden/BWDiffuse"));
    }

    // Postprocess the image
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (intensity == 0)
        {
            Graphics.Blit(source, destination);
            return;
        }

        material.SetFloat("_bwBlend", intensity);
        Graphics.Blit(source, destination, material);
    }
}