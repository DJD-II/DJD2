using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipCutscene : MonoBehaviour
{
    private float timer = 0;
    [SerializeField] GameObject player = null;
    [SerializeField] GameObject level = null;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            timer++;
        }
        else if (timer > 0)
            timer = 0;
        if (timer > 20)
        {
            gameObject.SetActive(false);
            level.SetActive(true);
            player.SetActive(true);
        }
    }
}
