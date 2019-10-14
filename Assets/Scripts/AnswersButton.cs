using UnityEngine;
using UnityEngine.UI;

public class AnswersButton : MonoBehaviour
{
    public PlayerAnswer pAnswer { get; private set; }
    public delegate void EventHandler(AnswersButton sender);
    public Text label;


    public void Initialize(PlayerAnswer panswer)
    {
        this.pAnswer = panswer;
        this.label.text = panswer.text;
    }

    public event EventHandler OnClick;

    public void Click()
    {
        OnClick?.Invoke(this);
    }

}
