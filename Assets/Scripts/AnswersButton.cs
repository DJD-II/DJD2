using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnswersButton : MonoBehaviour
{
    public PlayerAnswers PAnswer { get; private set; }
    public delegate void EventHandler(AnswersButton sender);
    public Text label;


    public void Initialize(PlayerAnswers panswer)
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
