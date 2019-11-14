using UnityEngine;
using UnityEngine.UI;

public class HackButton : MonoBehaviour
{
    [SerializeField]
    private Text number = null;
    private bool locked = false;

    public void ResetNumber()
    {
        number.text = "0";
        number.color = HUDSkinUtility.Skin.TextColor;
        locked = false;
    }

    public void Lock ()
    {
        locked = true;
    }

    public void Up()
    {
        if (locked)
            return;

        number.color = HUDSkinUtility.Skin.TextColor;

        int auxNumber = int.Parse(number.text) + 1;
        if (auxNumber > 9)
            auxNumber = 0;

        number.text = auxNumber.ToString();
    }

    public void Down()
    {
        if (locked)
            return;

        number.color = HUDSkinUtility.Skin.TextColor;

        int auxNumber = int.Parse(number.text) - 1;
        if (auxNumber < 0)
            auxNumber = 9;

        number.text = auxNumber.ToString();
    }

    public void SetColor(Color color)
    {
        number.color = color;
    }

    public int Number { get => int.Parse(number.text); }
}
