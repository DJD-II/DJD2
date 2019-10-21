using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{   
    public static string GetResolution()
    {
        string width = Screen.currentResolution.width.ToString();
        string height = Screen.currentResolution.height.ToString();

        return (width + " x " + height);
    }
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
