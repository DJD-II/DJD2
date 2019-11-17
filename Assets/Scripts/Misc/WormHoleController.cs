using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
sealed public class WormHoleController
{
    [SerializeField]
    private CirclesEffect effect = null;
    [SerializeField]
    private GameObject tunnelCamera = null;
    [SerializeField]
    private GameObject wormHoleTunnel = null;
    [SerializeField]
    private Animation shuttDown = null;
    [SerializeField]
    private AudioSource tunnelSFX = null;
    [SerializeField]
    private CameraShake wormHoleShake = null;
    [SerializeField]
    private AudioSource wormHoleEnterSFX = null;
    [SerializeField]
    private AudioSource wormHoleExitSFX = null;

    /// <summary>
    /// This method switches scenes. It takes the player through
    /// a wormhole so that he can reach the Cloud.
    /// </summary>
    /// <returns>IEnumerator.</returns>
    public IEnumerator SwitchToCloudScene(PlayerController controller)
    {
        GameInstance.HUD.EnableCrossHair(false);

        Spawner.SpawnAtID = 0;

        controller.ApplyHeal(new PointHeal(controller, controller.Hp.Max));
        controller.CanBeDamaged = false;

        GameInstance.Audio.FadeOut(
            AudioManager.Channel.Master, 1.5f);

        controller.HudSettings.HudsEnabled = false;

        effect.enabled = true;

        shuttDown.clip = shuttDown.GetClip("Shut Down");

        shuttDown.Play();
        while (shuttDown.isPlaying)
            yield return null;

        shuttDown.clip = shuttDown.GetClip("Turn On");

        wormHoleEnterSFX.Play();

        yield return new WaitForSecondsRealtime(1f);

        wormHoleTunnel.SetActive(true);
        tunnelCamera.SetActive(true);

        GameInstance.HUD.MaskScreen(true);

        wormHoleShake.Play(controller,
            tunnelCamera.GetComponent<Camera>(),
            1f);

        controller.StartCoroutine(GameInstance.HUD.FadeFromWhite(4f));
        shuttDown.Play();

        yield return new WaitForSecondsRealtime(1f);

        GameInstance.Save();

        AsyncOperation sceneLoadOp =
            SceneManager.LoadSceneAsync("ICloud");
        sceneLoadOp.allowSceneActivation = false;

        float passedTime = 0;

        while (sceneLoadOp.progress <= 0.8f)
        {
            passedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        yield return new WaitForSecondsRealtime(
            Mathf.Max(4f - passedTime, 0f));

        controller.StartCoroutine(DecreaseTunnelVolume());

        wormHoleExitSFX.Play();

        yield return GameInstance.HUD.FadeToWhite(2.2f);

        while (wormHoleExitSFX.isPlaying)
            yield return null;

        GameInstance.HUD.MaskScreen(false);

        sceneLoadOp.allowSceneActivation = true;
    }

    /// <summary>
    /// This method decreases the tunnel (Wormhole) sound effects
    /// while hes traveling though it.
    /// </summary>
    /// <returns>IEnumerator</returns>
    private IEnumerator DecreaseTunnelVolume(float speed = 2f)
    {
        while (tunnelSFX.volume > 0)
        {
            tunnelSFX.volume =
                Mathf.Lerp(tunnelSFX.volume,
                0,
                Time.deltaTime * speed);
            yield return null;
        }
    }
}
