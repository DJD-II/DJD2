using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LogoScreen : MonoBehaviour
{
    #region ---Parameters---
    [Header ("Warning")]
    [SerializeField] private RawImage backgroundEpilepsi = null;
    [SerializeField] private RawImage logoEpilepsi = null;
    [SerializeField] private TMP_Text textEpilepsi = null;
    [Header("Lusofona")]
    [SerializeField] private RawImage backgroundLusofona = null;
    [SerializeField] private RawImage logoLusofona = null;
    [SerializeField] private TMP_Text textLusofona = null;
    [Header ("Company")]
    [SerializeField] private RawImage backgroundCompany = null;
    [SerializeField] private RawImage logoCompany = null;
    [SerializeField] private TMP_Text textCompany = null;
    #endregion

    [SerializeField] private Color bgColor;
    [SerializeField] private Color logoColor;
    [SerializeField] private Color textColor;
    [SerializeField] private float timer = 0f;
    [SerializeField] private float duration = 3f;

    private float secondsElapsed;
    private bool coroutineIsRunning;

    private void Update()
    {
        timer += Time.deltaTime;
        secondsElapsed = timer % 60;
    
        if (!coroutineIsRunning && secondsElapsed > duration && backgroundEpilepsi.color.a > 0)
                StartCoroutine(Opacity(backgroundEpilepsi,logoEpilepsi, textEpilepsi));


        if (!coroutineIsRunning && secondsElapsed > duration * 2 && backgroundLusofona.color.a > 0)
                StartCoroutine(Opacity(backgroundLusofona, logoLusofona, textLusofona));

        else if (secondsElapsed > duration && secondsElapsed < duration * 3)
        {
            Scaler(logoLusofona.transform);
        }


        if (!coroutineIsRunning && secondsElapsed > duration * 3 && backgroundCompany.color.a > 0)
                StartCoroutine(Opacity(backgroundCompany, logoCompany, textCompany));

        else if (secondsElapsed > duration * 2)
        {
            Scaler(logoCompany.transform);
            Scaler(textCompany.transform);
        }

        if (!coroutineIsRunning && secondsElapsed > duration * 3.5f)
        {
            SceneManager.LoadSceneAsync("MainMenu");
        }
    }

    private void Scaler(Transform other)
    {
        other.localScale *= 1.0005f;
    }
    private IEnumerator Opacity(RawImage bg, RawImage logo, TMP_Text text)
    {
        bgColor = bg.color;
        logoColor = logo.color;
        textColor = text.color;

        coroutineIsRunning = true;
        while (bg.color.a > 0)
        {
            bgColor.a -= 0.01f;
            logoColor.a -= 0.01f;
            textColor.a -= 0.01f;
            
            bg.color = bgColor;
            logo.color = logoColor;
            text.color = textColor;
            yield return new WaitForSecondsRealtime(0.001f);
        }
        coroutineIsRunning = false;
    }
}
