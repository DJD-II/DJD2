using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// Responsible for most actions on the Main menu and Settings menu
/// </summary>
public class LoadSave : MonoBehaviour
{
    [Header("Load/save button")]
    // variable holding the save and load prefab button
    [SerializeField] private GameObject LoadSaveButtons = null;
    // Variable holding the tranform for the displaying the saved games
    [SerializeField] private Transform savedGamesContent = null;
    // Variable holding the load tab button
    [SerializeField] private GameObject loadMenu = null;
    // Variable for holding the confirm box prefab
    [SerializeField] private GameObject confirmBoxObject = null;
    // Variable for holding the start page of the main menu
    [SerializeField] private GameObject mainWindow = null;

    // Creates a variable for the confirm box script on
    private ConfirmBox confirmBox;
    // Creates a list of informations of the saves
    private List<FileInfo> infos;
    // Creates a string to hold the name of the save
    private string deleteName;

    /// <summary>
    /// Sets values to the variables created
    /// </summary>
    private void Awake()
    {
        // Gets the confirm box script and assigns it to the confirmBox variable
        confirmBox = confirmBoxObject.GetComponent<ConfirmBox>();
        // Gets all the save files on the IO class
        infos = new List<FileInfo>(IO.GetFilenames());
        // Sorts the save files by date
        SortFilesByModified();
    }

    /// <summary>
    /// Creates a confirm box to delelete the save
    /// </summary>
    /// <param name="sender"> the current saved game </param>
    public void Delete(SaveGameButton sender)
    {
        // Sets the deleteName to the name of the current saved game
        deleteName = sender.FileInfo.Name;

        // Sets the message of the confirm box and the value
        confirmBox.SetLabel("Are you sure you want to delete this save?", 2);

        // Creates the confirm box
        Instantiate(confirmBoxObject, gameObject.transform);
    }
    /// <summary>
    /// If the user clicked delete
    /// </summary>
    public void ConfirmDelete()
    {
        // uses the IO class to delete the save file by name
        IO.Delete(deleteName);
        // Gets the save files again
        infos = new List<FileInfo>(IO.GetFilenames());
        // Sorts the files by date
        SortFilesByModified();
        // Displays the buttons on screen
        InitializeButtons();
    }
    /// <summary>
    /// Loads the saved game
    /// </summary>
    /// <param name="sender"> the current saved game </param>
    public void Load(SaveGameButton sender)
    {
        // Uses the GameInstance to load the save clicked
        GameInstance.Load(sender.FileInfo.Name);
        // Closes the main menu.
        GameInstance.HUD.EnableMainMenu(false);
    }
    /// <summary>
    /// In case the user pressed continue
    /// </summary>
    public void OnClickContinue()
    {
        // Sorts the saved files by the date modified
        SortFilesByModified();
        // Uses the GameInstance to load the latest save file
        GameInstance.Load(infos[0].Name);
        // Disables the main menu
        GameInstance.HUD.EnableMainMenu(false);
    }
    /// <summary>
    /// Sorts the list of save files by chronological order
    /// </summary>
    private void SortFilesByModified()
    {
        // Sorts the list of save files by chronological order
        infos.Sort((FileInfo c1, FileInfo c2) => c1.LastWriteTimeUtc.
            CompareTo(c2.LastWriteTimeUtc));
        // Reverses the order to be from latest to oldest
        infos.Reverse();
    }
    /// <summary>
    /// Closes the load saved games menu
    /// </summary>
    public void CloseLoadSubMenu()
    {
        // Closes the load menu
        loadMenu.SetActive(false);
        // Enables the main screen of the main menu
        mainWindow.SetActive(true);
    }
    /// <summary>
    /// Creates all the saved game buttons
    /// </summary>
    public void InitializeButtons()
    {
        // Activates the load menu
        loadMenu.SetActive(true);
        // Checks if the main window is active
        if (mainWindow.activeSelf)
            // if it is disables it
            mainWindow.SetActive(false);

        // Destroys the children of the transform
        while (savedGamesContent.childCount > 0)
            DestroyImmediate(savedGamesContent.GetChild(0).gameObject);

        // Checks if the lenght of the saves is less than 1
        if (IO.GetFilenames().Length <= 0)
        {
            // Deletes the save/load buttons
            gameObject.GetComponent<StartMenuButtons>().DeleteLoadContinueButtons();
            // Closes the laod saves window
            CloseLoadSubMenu();
            // returns
            return;
        }

        // Runs a loop foreach save in infos list
        for (int i = 0; i < infos.Count - 1; i++)
        {
            // Creates a gameObject and instatiates a Load/save button
            GameObject go = Instantiate(LoadSaveButtons, savedGamesContent);
            // Creates a SaveGameButton and gets the script of go
            SaveGameButton button = go.GetComponent<SaveGameButton>();
            // initializes the button with the current info
            button.Initialize(infos[i]);
            // Adds load and delete to the event
            button.OnLoad += Load;
            button.OnDelete += Delete;
        }
    }
}
