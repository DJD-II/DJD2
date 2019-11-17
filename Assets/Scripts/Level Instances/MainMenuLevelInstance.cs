using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuLevelInstance : LevelInstance
{
    protected override void Start()
    {
        base.Start();

        GameInstance.HUD.EnableMainMenu(true);
    }
}
