using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Responsible for responding for the main menu buttons
/// </summary>
public class StartMenuButtons : MonoBehaviour
{
    // Variable for the confirm box prefab
    [SerializeField] private GameObject confirmBoxObject = null;
    // variable for the load game button
    [SerializeField] private GameObject loadGameButton = null;
    // variable for setting menu
    [SerializeField] private GameObject optionsSubMenu = null;
    // varible for the continue button
    [SerializeField] private GameObject continueButton = null;
    // vriable for the resolutions dropdown
    [SerializeField] private TMP_Dropdown resolution = null;
    // variable for the overall sound slider
    [SerializeField] private Slider musicSlider = null;
    // variable for the main window of the menu
    [SerializeField] private GameObject mainWindow = null;
    // variable for the dropdown with the quality settings
    [SerializeField] private TMP_Dropdown qualitySettings = null;

    // variable for the confirm box script
    private ConfirmBox confirmBox;
    // bool for the state of the fullscreen
    private bool fullscreen = true;
    // variable for holding the wanted resolution
    private Resolution wantedResolution;

    /// <summary>
    /// Sets values to the variables created
    /// </summary>
    private void Awake()
    {
        // Gets the script from the confirm box
        confirmBox = confirmBoxObject.GetComponent<ConfirmBox>();

        // Sets the resolution to the maximum resolution the monitor is capable
        wantedResolution = Screen.resolutions[Screen.resolutions.Length - 1];
        // Adds the options to the resolution dropdown
        AddDropDownOptions();
        // Adds the options to the quality dropdown
        AddDropDownQuality();
        // Checks if load and continue buttons should be visible
        DeleteLoadContinueButtons();
    }
    /// <summary>
    /// Adds the quality settings to the dropdown list
    /// </summary>
    private void AddDropDownQuality()
    {
        // Clears the dropdown list
        qualitySettings.ClearOptions();

        // Creates a list of strings for the new dropdowns
        List<string> dropdonws = new List<string>();

        // Runs a loop 6 times
        for (int i = 0; i < 5; i++)
        {
            // Adds the quality settings to the list
            dropdonws.Add(QualitySettings.names[i]);
        }
        // Sets the dropdown options to the list created
        qualitySettings.AddOptions(dropdonws);
    }
    /// <summary>
    /// Adds the resolution settings to the dropdown list
    /// </summary>
    private void AddDropDownOptions()
    {
        // Clears the dropdown list
        resolution.ClearOptions();

        // Creates a list of strings for the new dropdowns
        List<string> dropdowns = new List<string>();

        // Runs a loop for each resolution the monitor can handle
        for (int i = Screen.resolutions.Length - 1; i > 0; i--)
        {
            // Adds the resolution splitting the string by the '@' character
            dropdowns.Add(Screen.resolutions[i].ToString().Split('@')[0]);
        }
        // Sets the dropdown options to the list created
        resolution.AddOptions(dropdowns);
    }
    /// <summary>
    /// Creates a confirm box
    /// </summary>
    /// <param name="message"> the message to display </param>
    /// <param name="value"> the value it should use </param>
    private void OpenConfirmBox(string message, int value)
    {
        // Sets the confirm box to the value given
        confirmBox.SetLabel(message, value);
        // Instanciates a confirm box
        Instantiate(confirmBoxObject, gameObject.transform);
    }
    /// <summary>
    /// Deletes the load and continue buttons if there's no save files
    /// </summary>
    public void DeleteLoadContinueButtons()
    {
        // Checks if theres no save files
        if (IO.GetFilenames().Length <= 0)
        {
            // Deactivates the load and continue buttons
            loadGameButton.SetActive(false);
            continueButton.SetActive(false);
        }
    }
    /// <summary>
    /// Loads the first scene and disables the menu
    /// </summary>
    public void OnClickStart()
    {
        // Disables the main menu
        GameInstance.HUD.EnableMainMenu(false);
        // Enables the loading screen
        GameInstance.HUD.EnableLoadingScreen(true);
        // loads the Showcase scene
        SceneManager.LoadScene("Showcase");
    }
    /// <summary>
    /// Sets the resolution to the option choosen
    /// </summary>
    public void ChangeResolution()
    {
        // Creates a string and sets the value to the value choosen 
        string value = resolution.options[resolution.value].text;
        // Splits the string by the 'X' character
        string[] division = value.Split('x');

        // Sets the width to the first string on division
        wantedResolution.width = int.Parse(division[0]);
        // Sets the heigh to the second string on division
        wantedResolution.height = int.Parse(division[1]);
    }
    /// <summary>
    /// Changes the quality settings
    /// </summary>
    public void ChangeQuality()
    {
        // Creates a variable for holding the index of the quality settings
        int index = qualitySettings.value;
        // Set's the quality settings to the value choosen
        QualitySettings.SetQualityLevel(index);
    }
    /// <summary>
    /// Lowers the general volume
    /// </summary>
    public void ChangeAudio()
    {
        // Set's the volume of the audio to the value of the slider
        AudioListener.volume = musicSlider.value;
    }
    /// <summary>
    /// Change fullScreen to windowed and vice-versa
    /// </summary>
    public void ChangeFullscreen()
    {
        // Sets the fullscreen to the oposite value
        fullscreen = !fullscreen;
    }
    /// <summary>
    /// Enters function after clicking yes on the confirm box
    /// </summary>
    public void ConfirmAplly()
    {
        // Disables the optios menu
        ShowOptionsMenu(false);
        // Set's the resolution to the resolution set as well as fullscreen
        Settings.SetResolution(wantedResolution, fullscreen);
    }
    /// <summary>
    /// Shows or hides the options menu
    /// </summary>
    /// <param name="active"> bool for the state it should be </param>
    public void ShowOptionsMenu(bool active)
    {
        // Sets the state of the options menu to the value given
        optionsSubMenu.SetActive(active);
        // Sets the state of the main window the oposite of the value given
        mainWindow.SetActive(!active);
    }
    /// <summary>
    /// Sets the message and the value of the confirm box
    /// </summary>
    public void ApplySettings()
    {
        // Creates a confirm box with the message and the value
        OpenConfirmBox("Are you sure you want to keep these settings?", 1);
    }
    /// <summary>
    /// Sets the message and the value of the confirm box
    /// </summary>
    public void OnClickQuit()
    {
        // Creates a confirm box with the message and the value
        OpenConfirmBox("Are you sure you want to quit?", 0);
    }
}