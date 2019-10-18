using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class StartMenuButtons : MonoBehaviour
{
    [SerializeField] protected GameObject optionsSubMenu;
    [SerializeField] protected Dropdown resolution;
    [SerializeField] protected Slider musicSlider;
    [SerializeField] protected ConfirmBox confirmButton;
    [SerializeField] private GameObject confirmBox;

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
