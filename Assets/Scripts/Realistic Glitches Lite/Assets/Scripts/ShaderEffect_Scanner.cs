using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
sealed public class ShaderEffect_Scanner : MonoBehaviour {

    [SerializeField]
	private float area = 1;
	private Material material_a, material_b;

    public float Area { get { return area; } set { area = value; } }

	// Creates a private material used to the effect
	private void Awake ()
	{
		material_a = new Material( Shader.Find("Hidden/Shift") );
		material_b = new Material( Shader.Find("Hidden/Shift") );
	}

	// Postprocess the image
	private void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
	    material_a.SetFloat("_ValueY", area);
		material_b.SetFloat("_ValueY", -area);

		Graphics.Blit (source, source, material_a);
		Graphics.Blit (source, destination, material_b);
	}
}
