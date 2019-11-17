using UnityEngine;

sealed public class ShaderToTexture : MonoBehaviour
{

    [SerializeField]
    private ComputeShader shader = null;

    public ComputeShader Shader { get { return shader; } }

    private void RunShader()
    {
        int kernelHandle = shader.FindKernel("CSMain");

        RenderTexture tex = new RenderTexture(256, 256, 24);
        tex.enableRandomWrite = true;
        tex.Create();

        shader.SetTexture(kernelHandle, "Result", tex);
        shader.Dispatch(kernelHandle, 256 / 8, 256 / 8, 1);
    }

    // Use this for initialization
    private void Start()
    {
        RunShader();
    }
}
