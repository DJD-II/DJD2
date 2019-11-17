using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "HUD Skin", menuName = "New HUD Skin")]
public class HUDSkin : ScriptableObject
{
    [System.Serializable]
    public enum SpriteType : byte
    {
        Background = 0,
        Battery = 1,
        MinimapTop = 2,
        MinimapShadow = 3,
        MinimapCenter = 4,
        Panel = 5,
    }

    [System.Serializable]
    sealed public class ButtonSkin
    {
        [SerializeField]
        private Sprite normalButton = null;
        [SerializeField]
        private Sprite disabledButton = null;
        [SerializeField]
        private Sprite highlightedButton = null;
        [SerializeField]
        private Sprite selectedButton = null;

        public Sprite NormalButton { get => normalButton; }
        public Sprite DisabledButton { get => disabledButton; }
        public Sprite HighlightedButton { get => highlightedButton; }
        public Sprite SelectedButton { get => selectedButton; }
    }

    [System.Serializable]
    sealed public class SliderSkin
    {
        [SerializeField]
        private Sprite background = null;
        [SerializeField]
        private Sprite slider = null;
        [SerializeField]
        private Sprite handle = null;

        public Sprite Background { get => background; }
        public Sprite Slider { get => slider; }
        public Sprite Handle { get => handle; }
    }

    [System.Serializable]
    sealed public class ScrollViewSkin
    {
        [SerializeField]
        private Sprite background = null;
        [SerializeField]
        private SliderSkin handle = null;

        public Sprite Background { get => background; }
        public SliderSkin Handle { get => handle; }
    }


    [System.Serializable]
    sealed public class MapSkin
    {
        [SerializeField]
        private Sprite mapTop = null;
        [SerializeField]
        private Sprite mapShadow = null;
        [SerializeField]
        private Sprite mapBackground = null;

        public Sprite MapTop { get => mapTop; }
        public Sprite MapShadow { get => mapShadow; }
        public Sprite MapBackground { get => mapBackground; }
    }

    [SerializeField]
    private Color textColor = Color.white;
    [SerializeField]
    private Sprite background = null;
    [SerializeField]
    private Sprite battery = null;
    [Header("Map")]
    [SerializeField]
    private MapSkin map = null;
    [Header("Button")]
    [SerializeField]
    private ButtonSkin button = null;
    [Header("Panels")]
    [SerializeField]
    private Sprite panel = null;
    [Header("Sliders")]
    [SerializeField]
    private SliderSkin slider = null;
    [Header("Scroll View")]
    [SerializeField]
    private ScrollViewSkin scrollview = null;

    public Color TextColor { get => textColor; }
    public Sprite Background { get => background; }
    public Sprite Battery { get => battery; }
    public MapSkin Map { get => map; }
    public ButtonSkin Button { get => button; }
    public Sprite Panel { get => panel; }
    public SliderSkin Slider { get => slider; }
    public ScrollViewSkin Scrollview { get => scrollview; }
}
