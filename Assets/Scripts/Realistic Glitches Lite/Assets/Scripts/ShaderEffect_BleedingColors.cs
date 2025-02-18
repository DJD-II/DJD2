﻿using UnityEngine;

[ExecuteInEditMode]
public class ShaderEffect_BleedingColors : MonoBehaviour
{

    [SerializeField]
    private float intensity = 3;
    [SerializeField]
    private float shift = 0.5f;
    private Material material;
    private RenderTexture intermideateRT;

    public float Intensity { get { return intensity; } set { intensity = value; } }
    public float Shift { get { return shift; } set { shift = value; } }

    // Creates a private material used to the effect
    private void Awake()
    {
        material = new Material(Shader.Find("Hidden/BleedingColors"));
    }

    // Postprocess the image
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        intermideateRT = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);

        material.SetFloat("_Intensity", intensity);
        material.SetFloat("_ValueX", shift);
        Graphics.Blit(source, intermideateRT, material);
        Graphics.Blit(intermideateRT, destination);

        RenderTexture.ReleaseTemporary(intermideateRT);
    }
}
