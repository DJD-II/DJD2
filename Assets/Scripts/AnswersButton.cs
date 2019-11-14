using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnswersButton : MonoBehaviour
{
    public PlayerAnswer pAnswer { get; private set; }
    public delegate void EventHandler(AnswersButton sender);

    [SerializeField]
    private TMP_Text label = null;


    public void Initialize(PlayerAnswer panswer)
    {
        this.pAnswer = panswer;
        this.label.text = panswer.Text;
    }

    public event EventHandler OnClick;

    public void Click()
    {
        OnClick?.Invoke(this);
    }
}
