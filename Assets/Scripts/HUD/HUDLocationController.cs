using UnityEngine;
using UnityEngine.UI;

sealed public class HUDLocationController : MonoBehaviour
{
    [SerializeField] private Text locationLabel = null;
    [SerializeField] private float appearTime = 0.8f;
    [SerializeField] private float disapearTime = 2f;
    [SerializeField] private float showTime = 4f;
    private float timer = 0;

    private void Start()
    {
        GameInstance.GameState.LocationController.OnDiscoveredNewLocation +=
            OnDiscoveredNewLocation;
        GameInstance.GameState.OnPausedChanged += OnPausedChanged;
    }

    private void Update()
    {
        if (GameInstance.GameState.Paused)
            return;

        timer = Mathf.Max(timer - Time.deltaTime, 0f);

        Color c = locationLabel.color;
        c.a = timer > 0 ? 
              Mathf.Lerp(c.a, 1f, Time.deltaTime * appearTime) :
              Mathf.Lerp(c.a, 0f, Time.deltaTime * disapearTime);

        locationLabel.color = c;
    }

    private void OnDestroy()
    {
        GameInstance.GameState.LocationController.OnDiscoveredNewLocation -=
            OnDiscoveredNewLocation;
        GameInstance.GameState.OnPausedChanged -= OnPausedChanged;
    }

    private void OnPausedChanged(GameState sender)
    {
        locationLabel.gameObject.SetActive(!sender.Paused);
    }

    private void OnDiscoveredNewLocation(
        LocationController sender, 
        Location newLocation)
    {
        timer = showTime;
        locationLabel.text = 
            newLocation.ToString().Replace("_", " ");
    }
}