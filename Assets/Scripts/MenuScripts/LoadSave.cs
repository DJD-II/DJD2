using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LoadSave : MonoBehaviour
{
    [Header("Load/save button")]
    [SerializeField] private GameObject LoadSaveButtons = null;
    [SerializeField] private Transform savedGamesContent = null;
    [SerializeField] private GameObject loadMenu = null;
    [SerializeField] private GameObject confirmBoxObject = null;
    [SerializeField] private GameObject mainWindow = null;

    private ConfirmBox confirmBox;
    private List<FileInfo> infos;
    private string deleteName;

    private void Awake()
    {
        confirmBox = confirmBoxObject.GetComponent<ConfirmBox>();
        infos = new List<FileInfo>(IO.GetFilenames());
        SortFilesByModified();
    }

    public void Delete(SaveGameButton sender)
    {
        deleteName = sender.FileInfo.Name;

        confirmBox.SetLabel("Are you sure you want to delete this save?", 2);
        Instantiate(confirmBoxObject, gameObject.transform);
    }
    public void ConfirmDelete()
    {
        IO.Delete(deleteName); 
        infos = new List<FileInfo>(IO.GetFilenames());
        SortFilesByModified();
        InitializeButtons();
    }
    public void Load(SaveGameButton sender)
    {
        GameInstance.Load(sender.FileInfo.Name);
        gameObject.SetActive(false);
    }
    public void OnClickContinue()
    {
        SortFilesByModified();
        GameInstance.Load(infos[0].Name);
        gameObject.SetActive(false);
    }
    private void SortFilesByModified()
    {
        infos.Sort((FileInfo c1, FileInfo c2) => c1.LastWriteTimeUtc.CompareTo(c2.LastWriteTimeUtc));
        infos.Reverse();
    }
    public void CloseLoadSubMenu()
    {
        loadMenu.SetActive(false);
        mainWindow.SetActive(true);
    }
    public void InitializeButtons()
    {
        loadMenu.SetActive(true);
        if (mainWindow.activeSelf)
            mainWindow.SetActive(false);

        while (savedGamesContent.childCount > 0)
            DestroyImmediate(savedGamesContent.GetChild(0).gameObject);

        if (IO.GetFilenames().Length <= 0)
        {
            gameObject.GetComponent<StartMenuButtons>().DeleteLoadContinueButtons();
            CloseLoadSubMenu();
            return;
        }

        foreach (FileInfo info in infos)
        {
            GameObject go = Instantiate(LoadSaveButtons, savedGamesContent);
            SaveGameButton button = go.GetComponent<SaveGameButton>();
            button.Initialize(info);
            button.OnLoad += Load;
            button.OnDelete += Delete;
        }
    }
}
