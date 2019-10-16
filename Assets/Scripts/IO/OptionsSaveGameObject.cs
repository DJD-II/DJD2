using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
sealed public class OptionsSaveGameObject : SaveGame.Object
{
    public float                        _musicVolume,
                                        _effectsVolume,
                                        _announcerVolume;
    public int                          _qualitySettings;

    public OptionsSaveGameObject()
    {
    }

    public void Add(string key, KeyCode keyCode)
    {
        
    }
}