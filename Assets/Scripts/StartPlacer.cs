using UnityEngine;

sealed public class StartPlacer : MonoBehaviour
{
    private bool ignore = false;

    private void Awake()
    {
        GameInstance.OnLoad += OnLoad;
    }

    private void OnLoad()
    {
        ignore = GameInstance.Singleton.IsLoadingSavedGame;
    }

    private void OnDestroy()
    {
        GameInstance.OnLoad -= OnLoad;
    }

    private void Start()
    {
        if (ignore)
            return;

        PlayerStart[] playerStartPositions = LevelInstance.Singleton.StartPositions;
        PlayerStart startPosition = null;

        foreach (PlayerStart startPos in playerStartPositions)
            if (startPos.ID == GameInstance.Singleton.ToID)
            {
                startPosition = startPos;
                break;
            }

        if (startPosition == null)
        {
            Debug.LogWarning("No Player Start Position for " + gameObject.name);
            return;
        }

        startPosition.Spawn(gameObject.GetComponent<PlayerController>());
    }
}
