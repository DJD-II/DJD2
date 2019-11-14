using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Image))]
sealed public class HUDSpriteSelector : MonoBehaviour
{
    [SerializeField]
    private HUDSkin.SpriteType type = HUDSkin.SpriteType.Background;

    private void Awake()
    {
        Image imageComponent = GetComponent<Image>();
        switch (type)
        {
            case HUDSkin.SpriteType.Background:
                imageComponent.sprite = HUDSkinUtility.Skin.Background;
                break;
            case HUDSkin.SpriteType.Battery:
                imageComponent.sprite = HUDSkinUtility.Skin.Battery;
                break;
            case HUDSkin.SpriteType.MinimapCenter:
                imageComponent.sprite = HUDSkinUtility.Skin.Map.MapBackground;
                break;
            case HUDSkin.SpriteType.MinimapShadow:
                imageComponent.sprite = HUDSkinUtility.Skin.Map.MapShadow;
                break;
            case HUDSkin.SpriteType.MinimapTop:
                imageComponent.sprite = HUDSkinUtility.Skin.Map.MapTop;
                break;
            case HUDSkin.SpriteType.Panel:
                imageComponent.sprite = HUDSkinUtility.Skin.Panel;
                break;
        }
    }
}
