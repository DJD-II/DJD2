using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HackCommandButton : MonoBehaviour
{
    public delegate void EventHandler(HackCommandButton sender);

    public event EventHandler OnClicked;

    [SerializeField] Text text = null;
    public Hack.Point.Command Command { get; private set; }

    public void Initialize(Hack.Point.Command command)
    {
        Command = command;
        text.text = command.Text;
    }

    public void OnClick()
    {
        OnClicked?.Invoke(this);
    }
}
