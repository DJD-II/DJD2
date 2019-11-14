using UnityEngine;
using UnityEngine.UI;

sealed public class HUDMessageController : MonoBehaviour
{
    [SerializeField]
    private Text messageLabel = null;
    private float timer = 5f;

    public void Initialize(string message)
    {
        messageLabel.text = message;
    }

    private void Update()
    {
        timer = Mathf.Max(timer - Time.deltaTime, 0);
        if (timer == 0)
            Destroy(gameObject);
    }
}
