using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Options : StartMenuButtons
{
    private bool fullscreen = true;
    private Resolution wantedResolution;

    private void Awake()
    {
        /*
        for (int i = 0; i < resolution.options.Count; i++)
        {
            if (resolution.options[i].text == Settings.GetResolution())
            {
                resolution.options[i].text = resolution.options[i].text + " x (default)";
            }
        }*/
        
        wantedResolution = Screen.resolutions[Screen.resolutions.Length -1];
        ConfirmAplly();
        AddDropDownOptions();
    }

    private void AddDropDownOptions()
    {
        resolution.ClearOptions();
        List<string> dropdowns = new List<string>();
        for (int i = Screen.resolutions.Length -1; i > 0; i--)
        {
            dropdowns.Add(Screen.resolutions[i].ToString().Split('@')[0]);
        }
        resolution.AddOptions(dropdowns);
    }
    // Sets the resolution to the option chosen
    public void ChangeResolution()
    {
        string value = resolution.options[resolution.value].text;
        string[] division = value.Split('x');

        wantedResolution.width = int.Parse(division[0]);
        wantedResolution.height = int.Parse(division[1]);
    }

    // Lowers the general volume
    public void ChangeAudio()
    {
        AudioListener.volume = musicSlider.value;
    }

    // Change fullScreen to windowed and vice-versa
    public void ChangeFullscreen()
    {
        fullscreen = !fullscreen;
    }

    #region ---Cancel/Apply buttons---
    public void ApplySettings()
    {
        confirmButton.OnCancel(true);
        confirmButton.SetLabel("Are you sure you want to keep these settings?");
        confirmButton.value = 1;
    }

    public void Cancel()
    {
        optionsSubMenu.SetActive(false);
    }
    #endregion

    // Enters function after clicking yes on the confirm box
    public void ConfirmAplly()
    {
        confirmButton.OnCancel(false);
        optionsSubMenu.SetActive(false);
        Settings.SetResolution(wantedResolution, fullscreen);
        Settings.SetFullscreen(fullscreen);
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSecondsRealtime(10);
    }
}
