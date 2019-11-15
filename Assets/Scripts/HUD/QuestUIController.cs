using TMPro;
using UnityEngine;

sealed public class QuestUIController : MonoBehaviour
{
    [SerializeField]
    private GameObject panel = null;
    [SerializeField]
    private TextMeshProUGUI titleLabel = null;
    [SerializeField]
    private TextMeshProUGUI descriptionLabel = null;
    private float timer = 0f;

    private void Awake()
    {
        gameObject.transform.position = new Vector3(-100, gameObject.transform.position.y, 0f);
    }

    private void Start()
    {
        GameInstance.GameState.QuestController.OnQuestAdded += OnQuestAdded;
        GameInstance.GameState.OnPausedChanged += (GameState sender) =>
        {
            if (sender.Paused)
                panel.SetActive(false);
            else
                panel.SetActive(true);
        };
    }

    private void OnQuestAdded(QuestController sender, QuestID quest)
    {
        timer = 15f;
        if (titleLabel != null)
            titleLabel.text = quest.quest.name;

        if (descriptionLabel != null)
            descriptionLabel.text = quest.quest.description;
    }

    private void Update()
    {
        timer = Mathf.Max(timer - Time.deltaTime, 0);

        if (timer == 0)
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, new Vector3(-100, gameObject.transform.position.y, 0f), Time.unscaledDeltaTime);
        else
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, new Vector3(0, gameObject.transform.position.y, 0f), Time.deltaTime);
    }
}
