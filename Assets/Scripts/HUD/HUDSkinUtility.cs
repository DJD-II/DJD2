using UnityEngine;

public static class HUDSkinUtility
{
    private static HUDSkin[] Skins { get; } = Resources.LoadAll<HUDSkin>("HUD Skins");
    private static uint currentSkin = 0;

    public static uint CurrentSkin
    {
        get => currentSkin;

        set
        {
            currentSkin = (uint)System.Math.Max(System.Math.Min(value, Skins.Length - 1), 0);
        }
    }
    public static HUDSkin Skin { get => Skins[currentSkin]; }
}
