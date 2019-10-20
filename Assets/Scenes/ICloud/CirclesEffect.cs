using UnityEngine;

[ExecuteInEditMode]
public class CirclesEffect : MonoBehaviour
{
    [SerializeField]
    private Material material = null;
    [SerializeField]
    private Material material2 = null;
    [SerializeField]
    private float scaleX = 1f;
    [SerializeField]
    private float scaleY = 1f;
    [SerializeField]
    private AudioSource shutDownSFX = null;

    public void PLaySFX()
    {
        shutDownSFX.Play();
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        material.SetFloat("_ScaleX", Mathf.Max(scaleX, 0.00000001f));
        material.SetFloat("_ScaleY", Mathf.Max(scaleY, 0.00000001f));

        Graphics.Blit(src, dest, material2, 0);
        Graphics.Blit(src, dest, material, 0);
        

    }
}