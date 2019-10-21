using System.Collections;
using UnityEngine;

public class ICloudLevelInstance : LevelInstance
{
    [SerializeField]
    private PlayerController controller = null;
    [SerializeField]
    private AudioSource ambientMusic = null;
    private float startVolume;

    private void Start()
    {
        GameInstance.HUD.TalkUIController.OnAnswered += (TalkUIController sender, PlayerAnswer answer) =>
        {
            if (answer.ID == 101)
            {
                GameInstance.HUD.EnablePastLife(true);
            }
        };
        startVolume = ambientMusic.volume;
        ambientMusic.volume = 0;
        StartCoroutine(StartScene());
    }

    private IEnumerator StartScene ()
    {
        yield return new WaitForSecondsRealtime(2f);
        StartCoroutine(AplifyMusic());
        controller.CanControl = true;
        yield return GameInstance.HUD.FadeFromWhite();
    }

    private IEnumerator AplifyMusic()
    {
        ambientMusic.Play();
        while (ambientMusic.volume < 1)
        {
            ambientMusic.volume = Mathf.Lerp(ambientMusic.volume, startVolume, Time.unscaledDeltaTime * 0.2f);
            yield return  null;
        }
    }
}
