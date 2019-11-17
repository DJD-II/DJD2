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
        material.SetFloat("_UnscaledDeltaTime", Time.unscaledTime);
    }
}
