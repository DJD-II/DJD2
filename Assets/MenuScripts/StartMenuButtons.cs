using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuButtons : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButon;

    public void OnClickStart()
    {
        SceneManager.LoadScene("Showcase");
    }
    public void OnClickOptions()
    {

    }

    public void OnClickQuit()
    {
        startButton.interactable = false;
        optionsButton.interactable = false;
        quitButon.interactable = false;


        Application.Quit();
        
    }
}
