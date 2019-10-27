using UnityEngine;
using UnityEngine.UI;

public class ConfirmBox : MonoBehaviour
{
    private StartMenuButtons options;
    private LoadSave save;
    [SerializeField] private Text label = null;
    [SerializeField] private int value;

    private void Start()
    {
        options = GameObject.Find("MainMenu").GetComponent<StartMenuButtons>();
        save = GameObject.Find("MainMenu").GetComponent<LoadSave>();
    }
    public void SetLabel(string label, int value)
    {
        this.label.text = label;
        this.value = value;
    }
    public void OnCancel()
    {
        Destroy(gameObject);
    }
    public void OnConfirm()
    {
        switch (value)
        {
            case 0: Application.Quit(); break;
            case 1: options.ConfirmAplly(); break;
            case 2: save.ConfirmDelete(); break;
        }
        OnCancel();
    }
}
