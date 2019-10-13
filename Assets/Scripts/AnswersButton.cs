using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnswersButton : MonoBehaviour
{
    public PLAYERanswer pAnswer { get; private set; }
    public delegate void EventHandler(AnswersButton sender);
    public Text label;


    public void Initialize(PLAYERanswer panswer)
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
