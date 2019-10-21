using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class ShaderEffect_Unsync : MonoBehaviour {

	public enum MovementType
    {
        JUMPING_FullOnly,
        SCROLLING_FullOnly, STATIC
    };

    [SerializeField]
    private MovementType movement = MovementType.STATIC;
    [SerializeField]
	private float speed = 1;
	private float position = 0;
	private Material material;

    public float Speed { get { return speed; } set { speed = value; } }
    public MovementType Movement { get { return movement; } set { movement = value; } }

    private void Awake ()
	{
		material = new Material( Shader.Find("Hidden/VUnsync") );
	}

    private void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		position = speed * 0.1f;

		material.SetFloat("_ValueX", position);
		Graphics.Blit (source, destination, material);
	}
}
