using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (AudioSource))]
sealed public class MusicAdvance : MonoBehaviour
{
    [Range(0f, 1f)]
    [SerializeField]
    private float minAdvance = 0.2f;
    [Range(0f, 1f)]
    [SerializeField]
    private float maxAdvance = 0.2f;
    [SerializeField]
    private bool autoPlay = true;

    private void Start()
    {
        AudioSource source = GetComponent<AudioSource>();

        float min = Mathf.Min(minAdvance, maxAdvance);
        float max = Mathf.Max(minAdvance, maxAdvance);
        float random = Random.Range(min, max);

        source.timeSamples = Mathf.CeilToInt(random * source.clip.samples);
        Debug.Log("Time = " + source.timeSamples + " Random = " + random);

        if (autoPlay)
            source.Play();
    }
}
