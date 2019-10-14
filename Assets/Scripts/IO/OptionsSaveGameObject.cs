using UnityEngine;

[System.Serializable]
sealed public class OptionsSaveGameObject : IO.Object
{
    public float _musicVolume,
                                        _effectsVolume,
                                        _announcerVolume;
    public int _qualitySettings;

    public OptionsSaveGameObject()
    {
    }

    public void Add(string key, KeyCode keyCode)
    {

    }
}