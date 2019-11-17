using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{   
    public static void SetResolution(Resolution res, bool fullscreen)
    {
        Screen.SetResolution(res.width,res.height, fullscreen);
        Debug.Log("Applied settings");
    }

    public static void SetFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
    }
}
