using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
sealed public class ShaderEffect_CorruptedVram : MonoBehaviour {
    [SerializeField]
	private float shift = 10;
	private Texture texture;
	private Material material;

    public float Shift { get { return shift; } set { shift = value; } }

    private void Awake ()
	{
		material = new Material( Shader.Find("Hidden/Distortion") );
		texture = Resources.Load<Texture>("Checkerboard-big");
	}
		
	private void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		material.SetFloat("_ValueX", shift);
		material.SetTexture("_Texture", texture);
		Graphics.Blit (source, destination, material);
	}
}
