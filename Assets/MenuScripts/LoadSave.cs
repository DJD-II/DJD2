using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.IO;

public class LoadSave : MonoBehaviour
{
    [Header("Load/save button")]
    [SerializeField] private GameObject LoadSaveButtons;
    [SerializeField] private Transform savedGamesContent;
    [SerializeField] private GameObject loadMenu;
    [SerializeField] private GameObject confirmBoxObject;

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
    }
    public void InitializeButtons()
    {
        loadMenu.SetActive(true);

        while (savedGamesContent.childCount > 0)
            DestroyImmediate(savedGamesContent.GetChild(0).gameObject);

        if (IO.GetFilenames().Length <= 0)
        {
            gameObject.GetComponent<StartMenuButtons>().DeleteLoadContinue();
            loadMenu.SetActive(false);
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
