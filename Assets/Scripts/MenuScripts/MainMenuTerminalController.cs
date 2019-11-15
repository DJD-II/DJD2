using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuTerminalController : MonoBehaviour
{
    [SerializeField] private TMP_Text terminalText = null;
    [SerializeField] private Scrollbar bar = null;
    [SerializeField] private string[] code = new string[0];
    [SerializeField] private float speed = 1;

    [SerializeField] private RawImage tempFadeImage = null;

    void Start()
    {
        terminalText = GetComponent<TMP_Text>();

        StartCoroutine(TempFade());
        StartCoroutine(Log());
    }
    private void OnEnable()
    {
        StartCoroutine(Log());
    }
    private IEnumerator TempFade()
    {
        Color cor = new Color(0, 0, 0);
        while (tempFadeImage.color.a > 0)
        {
            cor.a -= 0.01f;
            tempFadeImage.color = cor;
            yield return new WaitForSecondsRealtime(0.01f);
        }
        tempFadeImage.gameObject.SetActive(false);
    }
    private IEnumerator Log()
    {
        foreach (string a in code)
        {
            terminalText.text += "\n" + a;
            bar.value = 0;
            yield return new WaitForSecondsRealtime(speed);
        }
        terminalText.text = "";
        StartCoroutine(Log());
    }
}
