using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ICloudLevelInstance : LevelInstance
{
    private void Start()
    {
        StartCoroutine(GameInstance.HUD.FadeFromWhite());
    }
}
