using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class HUDSkinButtonSelector : MonoBehaviour
{
    private void Awake()
    {
        Button b = GetComponent<Button>();
        HUDSkin skin = HUDSkinUtility.Skin;
        SpriteState state = b.spriteState;
        state.highlightedSprite = skin.Button.HighlightedButton;
        state.pressedSprite = skin.Button.NormalButton;
        state.disabledSprite = skin.Button.DisabledButton;
        state.selectedSprite = skin.Button.SelectedButton;
        b.image.sprite = skin.Button.NormalButton;

        b.spriteState = state;
    }
}
