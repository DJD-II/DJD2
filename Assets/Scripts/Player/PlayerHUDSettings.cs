using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
sealed public class PlayerHUDSettings
{
    [SerializeField]
    private bool hudsEnabled = true;
    [SerializeField]
    private GameObject huds = null;
    [SerializeField]
    private Image batteryImage = null;
    [SerializeField]
    private Transform messageContents = null;
    [SerializeField]
    private GameObject messagePrefab = null;

    public bool HudsEnabled
    {
        get => hudsEnabled;

        set
        {
            hudsEnabled = value;
            huds.SetActive(!GameInstance.GameState.Paused && value);
        }
    }

    /// <summary>
    /// This method updates the Heads up display battery image.
    /// The HUD Battery image gets shorter or bigger,
    /// as the players battery decreases or increases respectively. 
    /// </summary>
    public void UpdateLifeHUD(PlayerController controller)
    {
        batteryImage.fillAmount = controller.Hp.Scalar;
    }

    public void OnPauseChanged(GameState sender)
    {
        huds.SetActive(!sender.Paused && hudsEnabled);
    }

    /// <summary>
    /// This method is called so that it can pop messages on the right top 
    /// corner of the screen.
    /// When the player is low on bobby pins, or any other message useful
    /// then it is shown by calling this method.
    /// </summary>
    /// <param name="message">The message to be printed.</param>
    public void PopMessage (string message)
    {
        GameObject go = GameObject.Instantiate(
            messagePrefab,
            messageContents);

        HUDMessageController messageController =
            go.GetComponent<HUDMessageController>();

        messageController.Initialize(message);
    }
}
