using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
sealed public class ShaderEffect_Tint : MonoBehaviour {
    [SerializeField]
    private float y = 1;
    [SerializeField]
    private float u = 1;
    [SerializeField]
    private float v = 1;
//	public bool swapUV = false;
	private Material material;

    public float Y { get { return y; } set { y = value; } }
    public float U { get { return u; } set { u = value; } }
    public float V { get { return v; } set { v = value; } }

    // Creates a private material used to the effect
    private void Awake ()
	{
		material = new Material( Shader.Find("Hidden/Tint") );
	}

    // Postprocess the image
    private void OnRenderImage (RenderTexture source, RenderTexture destination)
	{

		material.SetFloat("_ValueX", y);
		material.SetFloat("_ValueY", u);
		material.SetFloat("_ValueZ", v);

		Graphics.Blit (source, destination, material);
	}
}
