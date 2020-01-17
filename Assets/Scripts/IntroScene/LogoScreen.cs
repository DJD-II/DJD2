using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoScreen : MonoBehaviour
{
    [SerializeField] private List<CanvasGroup> logos = new List<CanvasGroup>();
    private WaitForSecondsRealtime elapsed = new WaitForSecondsRealtime(0.001f);
    private WaitForSecondsRealtime held = new WaitForSecondsRealtime(5);
    private bool coroutineIsRunning;
    private int index = 0;

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            StopCoroutine(Opacity(null));
            index = Mathf.Max(2, index++);
        }
        if (!coroutineIsRunning)
        {
            StartCoroutine(Opacity(logos[index]));
        }
    }

    private IEnumerator Opacity(CanvasGroup group)
    {
        coroutineIsRunning = true;

        while (group.alpha > 0)
        {
            group.alpha -= 0.005f;
            yield return elapsed;
        }

        if (index < logos.Count - 1)
        {
            yield return held;
            index++;
        }
        else
            SceneManager.LoadSceneAsync("MainMenu");
        coroutineIsRunning = false;
    }
}