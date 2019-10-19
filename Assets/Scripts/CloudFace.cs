using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudFace : MonoBehaviour
{
    [SerializeField]
    Material material = null;

    public void SetAlpha(float alpha)
    {
        material.SetFloat("_Alpha", alpha);
    }

    void Update()
    {
        material.SetFloat("_UnscaledTime", Time.unscaledTime);    
    }
}
