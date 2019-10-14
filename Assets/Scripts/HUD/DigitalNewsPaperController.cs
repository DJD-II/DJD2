using UnityEngine;

public class DigitalNewsPaperController : MonoBehaviour
{
    public void Close()
    {
        GameInstance.HUD.EnableDigitalNewsPaper(false);
    }
}
