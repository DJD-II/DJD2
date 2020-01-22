using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Responsible for switcheing the various logos on the start
/// </summary>
public class LogoScreen : MonoBehaviour
{
    // Creates a list to store the canvas groups 
    [SerializeField] private List<CanvasGroup> logos = new List<CanvasGroup>();
    // Creates a WaitForSecondsRealtime variable with the value of 0.001 second
    private readonly WaitForSecondsRealtime elapsed = 
        new WaitForSecondsRealtime(0.001f);
    // Creates a WaitForSecondsRealtime variable with the value of 5 seconds
    private readonly WaitForSecondsRealtime held = 
        new WaitForSecondsRealtime(5);

    // Bool for checking if the couroutine is running or not
    private bool coroutineIsRunning;
    // The index of the list it should send to the courotine
    private int index = 0;

    /// <summary>
    /// Checks if the user tried to skip and starts the couroutine if it's not
    /// running
    /// </summary>
    private void Update()
    {
        // Checks if the user pressed a button
        if (Input.anyKeyDown)
        {
            // Stops the Opacity couroutine
            StopCoroutine(Opacity(null));
            // Adds one to the index
            index = Mathf.Max(2, index++);
        }
        // Checks if the couroutine is stopped
        if (!coroutineIsRunning)
        {
            // Starts the Opacity couroutine
            StartCoroutine(Opacity(logos[index]));
        }
    }
    /// <summary>
    /// Lowers the Opacity of the canvas group making a fade effect
    /// </summary>
    /// <param name="group"> the Canvas group to reduce the opacity </param>
    /// <returns> a IEnumerator </returns>
    private IEnumerator Opacity(CanvasGroup group)
    {
        // Sets the bool for the couroutine running to true
        coroutineIsRunning = true;

        // Loops while the tranparency of the group is bigger than 0
        while (group.alpha > 0)
        {
            // Reduces the transparency of the group by a value 
            group.alpha -= 0.005f;
            // Waits for the the amount set in the elapsed variable
            yield return elapsed;
        }

        // Checks if the index is not at the end of the list
        if (index < logos.Count - 1)
        {
            // Waits for the set amount in the held variable
            yield return held;
            // increments index by one
            index++;
        }
        // If it's at the end
        else
            // Loads the MainMenu scene
            SceneManager.LoadSceneAsync("MainMenu");

        // Sets the couroutine running to false
        coroutineIsRunning = false;
    }
}