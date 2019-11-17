using UnityEngine;

/// <summary>
/// This Script Makes an audio source start at a random clip position.
/// Given a min and max (normalized) position. The clip will start at a random
/// with a random offset.
/// </summary>
[RequireComponent(typeof(AudioSource))]
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
        //Gets the Audio Source Component
        AudioSource source = GetComponent<AudioSource>();

        //Calculate the true min and max values
        float min = Mathf.Min(minAdvance, maxAdvance);
        float max = Mathf.Max(minAdvance, maxAdvance);
        //Generate a random number between min and max
        float random = Random.Range(min, max);

        //Set the start position of the Audio Source at a random position.
        //The position is given by a random number (normalized).
        source.timeSamples = Mathf.CeilToInt(random * source.clip.samples);

        //Play the Audio source
        if (autoPlay)
            source.Play();
    }
}
