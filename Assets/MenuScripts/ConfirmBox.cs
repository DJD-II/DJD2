using UnityEngine;
using UnityEngine.UI;

public class ConfirmBox : MonoBehaviour
{
    [SerializeField] private Options options;
    [SerializeField] private Text label;
    [SerializeField] public int value;

    public void SetLabel(string label)
    {
        this.label.text = label;
    }
    public void OnCancel(bool activate)
    {
        gameObject.SetActive(activate);
    }
    public void OnConfirm()
    {
        switch (value)
        {
            case 0: Application.Quit(); break;
            case 1: options.ConfirmAplly() ; break;
        }
    }
}
