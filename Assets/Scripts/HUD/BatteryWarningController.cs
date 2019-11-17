using UnityEngine;
using UnityEngine.UI;

sealed public class BatteryWarningController : MonoBehaviour
{
    [SerializeField]
    private Text warningLabel = null;
    [SerializeField]
    private Image warningImage = null;
    [SerializeField]
    private AnimationCurve alphaIntensity = null;
    [SerializeField]
    private PlayerController controller = null;

    private void Update()
    {
        if (controller.Hp.Scalar > 0.3f)
        {
            warningImage.gameObject.SetActive(false);
            warningLabel.gameObject.SetActive(false);
        }
        else
        {
            warningImage.gameObject.SetActive(true);
            warningLabel.gameObject.SetActive(true);
            float intensity = alphaIntensity.Evaluate(Time.time);
            warningLabel.color = new Color(warningLabel.color.r, warningLabel.color.g, warningLabel.color.b, intensity);
            warningImage.color = new Color(warningImage.color.r, warningImage.color.g, warningImage.color.b, intensity);
        }
    }
}
