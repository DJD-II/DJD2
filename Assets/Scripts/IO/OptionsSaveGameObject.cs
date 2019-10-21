
[System.Serializable]
sealed public class OptionsSaveGameObject : IO.Object
{
    public float MusicVolume { get; set; }
    public float EffectsVolume { get; set; }
    public float VoiceVolume { get; set; }
    public int QualitySettings { get; set; }

    public OptionsSaveGameObject()
    {
    }
}