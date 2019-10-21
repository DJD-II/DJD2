using UnityEngine;

sealed public class DigitalNewsPaperController : MonoBehaviour
{
    public void Close()
    {
        GameInstance.HUD.EnableDigitalNewsPaper(false);
    }
}
