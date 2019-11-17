using UnityEngine;

[ExecuteInEditMode]
public class ShaderEffect_Unsync : MonoBehaviour
{

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
    private RenderTexture intermideateRT = null;

    public float Speed { get { return speed; } set { speed = value; } }
    public MovementType Movement { get { return movement; } set { movement = value; } }

    private void Awake()
    {
        material = new Material(Shader.Find("Hidden/VUnsync"));
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        intermideateRT = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);
        position = speed * 0.1f;

        material.SetFloat("_ValueX", position);
        Graphics.Blit(source, intermideateRT, material);
        Graphics.Blit(intermideateRT, destination);
        RenderTexture.ReleaseTemporary(intermideateRT);
    }
}
