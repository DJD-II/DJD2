using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class StartMenuButtons : MonoBehaviour
{
    [SerializeField] protected GameObject optionsSubMenu = null;
    [SerializeField] protected Dropdown resolution = null;
    [SerializeField] protected Slider musicSlider = null;
    [SerializeField] protected ConfirmBox confirmButton = null;
    [SerializeField] private GameObject confirmBox = null;

    private void Start()
    {
        confirmButton = confirmBox.GetComponent<ConfirmBox>();
    }
    public void OnClickStart()
    {
        gameObject.SetActive(false);
        GameInstance.HUD.EnableLoadingScreen(true);
        SceneManager.LoadScene("Showcase");
    }
    public void OnClickLoad()
    {

    }
    public void OnClickOptions()
    {
        optionsSubMenu.SetActive(true);
    }
    public void OnClickQuit()
    {
        confirmButton.OnCancel(true);
        confirmButton.SetLabel("Are you sure you want to quit?");
        confirmButton.value = 0;
    }
}
