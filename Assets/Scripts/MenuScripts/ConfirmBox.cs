using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Responsible for displaying a confirm box
/// </summary>
public class ConfirmBox : MonoBehaviour
{
    // Creates a StartMenuButtons variable
    private StartMenuButtons options;
    // Creates a LoadSave variable
    private LoadSave save;
    // Creates a variable for holding the message
    [SerializeField] private TMP_Text label = null;
    // Creates a variable for holding a value
    [SerializeField] private int value;

    /// <summary>
    /// called on the frame when a script is enabled
    /// Finds the main menu to get the needed scripts
    /// </summary>
    private void Start()
    {
        // Finds in scene the Main Menu and gets the StartMenuButtons script
        options = GameObject.Find("Main Menu").GetComponent<StartMenuButtons>();
        // Finds in scene the Main Menu and gets the LoadSave script
        save = GameObject.Find("Main Menu").GetComponent<LoadSave>();
    }
    /// <summary>
    /// Sets the description message and the value of this script to the 
    /// ones given
    /// </summary>
    /// <param name="label"> the wanted message </param>
    /// <param name="value"> the wanted action value </param>
    public void SetLabel(string label, int value)
    {
        this.label.text = label;
        this.value = value;
    }
    /// <summary>
    /// In case the user presses cancel
    /// </summary>
    public void OnCancel()
    {
        // Destroys itself
        Destroy(gameObject);
    }
    /// <summary>
    /// Performs an actiona ccording to the value given
    /// </summary>
    public void OnConfirm()
    {
        // Switch statement according to the value
        switch (value)
        {
            // Closes the game in case the value is 0
            case 0: Application.Quit(); break;
            // Enters the method ConfirmApply on StartMenuButtons
            case 1: options.ConfirmAplly(); break;
            // Confirms the deletion on LoadSave
            case 2: save.ConfirmDelete(); break;
        }
        // Destroys this game object
        OnCancel();
    }
}
