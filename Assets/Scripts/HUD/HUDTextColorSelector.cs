using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
sealed public class HUDTextColorSelector : MonoBehaviour
{
    private void Awake()
    {
        Text text = GetComponent<Text>();
        text.color = HUDSkinUtility.Skin.TextColor;
    }
}
