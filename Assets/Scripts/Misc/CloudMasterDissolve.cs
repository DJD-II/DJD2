using System.Collections;
using UnityEngine;

public class CloudMasterDissolve : MonoBehaviour
{
    [SerializeField]
    private Material dissolveMaterial = null;
    [SerializeField]
    private Material matrixMaterial = null;
    private bool shouldMove = false;

    private void Start()
    {
        dissolveMaterial.SetFloat("_Progress", 1f);
        matrixMaterial.SetFloat("_Alpha", 1f);

        GameInstance.HUD.OnTalkBegin += (HUD sender) =>
        {
            shouldMove = true;
        };

        GameInstance.HUD.OnTalkClose += (HUD sender) =>
        {
            shouldMove = false;
        };

        GameInstance.HUD.TalkUIController.OnAnswered += (TalkUIController sender, PlayerAnswer answer) =>
        {
            if (answer.ID == 101)
                StartCoroutine(Dissolve());
        };
    }

    private void Update()
    {
        if (!shouldMove)
            return;

        transform.localPosition = Vector3.Lerp(transform.localPosition,
            new Vector3(((Mathf.PerlinNoise(Time.unscaledTime * 0.2f, 10) * 2) - 1) * 0.3f,
                        ((Mathf.PerlinNoise(Time.unscaledTime * 0.2f, 2) * 2) - 1) * 0.3f,
                        transform.localPosition.z),
            Time.unscaledDeltaTime * 2f);
    }

    private IEnumerator Dissolve()
    {
        GetComponent<Collider>().enabled = false;

        float d = 1,
              e = 1;
        while (d > 0)
        {
            d = Mathf.Lerp(d, 0, Time.unscaledDeltaTime);
            e = Mathf.Lerp(e, 0, Time.unscaledDeltaTime * 2f);
            dissolveMaterial.SetFloat("_Progress", d);
            matrixMaterial.SetFloat("_Alpha", e);
            yield return null;
        }

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        dissolveMaterial.SetFloat("_Progress", 1f);
        matrixMaterial.SetFloat("_Alpha", 1f);

    }
}
