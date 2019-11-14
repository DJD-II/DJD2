using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StartMenuButtons : MonoBehaviour
{
    [SerializeField] private GameObject confirmBoxObject = null;
    [SerializeField] private GameObject loadGameButton = null;
    [SerializeField] private GameObject optionsSubMenu = null;
    [SerializeField] private GameObject continueButton = null;
    [SerializeField] private TMP_Dropdown resolution = null;
    [SerializeField] private Slider musicSlider = null;
    [SerializeField] private GameObject mainWindow = null;

    
    private ConfirmBox confirmBox;
    private bool fullscreen = true;
    private Resolution wantedResolution;

    private void Awake()
    {
        confirmBox = confirmBoxObject.GetComponent<ConfirmBox>();

        wantedResolution = Screen.resolutions[Screen.resolutions.Length - 1];
        ConfirmAplly();
        AddDropDownOptions();
        DeleteLoadContinueButtons();
    }
    private void AddDropDownOptions()
    {
        resolution.ClearOptions();

        List<string> dropdowns = new List<string>();
        for (int i = Screen.resolutions.Length - 1; i > 0; i--)
        {
            dropdowns.Add(Screen.resolutions[i].ToString().Split('@')[0]);
        }
        resolution.AddOptions(dropdowns);
    }
    private void OpenConfirmBox(string message, int value)
    {
        confirmBox.SetLabel(message, value);
        Instantiate(confirmBoxObject, gameObject.transform);
    }
    public void DeleteLoadContinueButtons()
    {
        if (IO.GetFilenames().Length <= 0)
        {
            loadGameButton.SetActive(false);
            continueButton.SetActive(false);
        }
    }
    public void OnClickStart()
    {
        gameObject.SetActive(false);
        GameInstance.HUD.EnableLoadingScreen(true);
        SceneManager.LoadScene("Showcase");
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
    // Enters function after clicking yes on the confirm box
    public void ConfirmAplly()
    {
        ShowOptionsMenu(false);
        Settings.SetResolution(wantedResolution, fullscreen);
        Settings.SetFullscreen(fullscreen);
    }
    public void ShowOptionsMenu(bool active)
    {
        optionsSubMenu.SetActive(active);
        mainWindow.SetActive(!active);
    }
    public void ApplySettings()
    {
        OpenConfirmBox("Are you sure you want to keep these settings?", 1);
    }
    public void OnClickQuit()
    {
        OpenConfirmBox("Are you sure you want to quit?", 0);
    }
}
