using UnityEngine;
using UnityEngine.UI;

public class AnswersButton : MonoBehaviour
{
    public PlayerAnswer PAnswer { get; private set; }
    public delegate void EventHandler(AnswersButton sender);

    [SerializeField]
    private Text label = null;
    public Text Label { get => label; }


    public void Initialize(PlayerAnswer panswer)
    {
        this.PAnswer = panswer;
        this.label.text = panswer.text;
    }

    public event EventHandler OnClick;

    public void Disable()
    {
        GetComponent<Button>().interactable = false;
    }

    public void Click()
    {
        OnClick?.Invoke(this);
    }

}
