using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

sealed public class QuestMessageController : MonoBehaviour
{
    private struct QuestMessage
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public bool Completed { get; private set; }

        public QuestMessage (string name, string description, bool completed)
        {
            Name = name;
            Description = description;
            Completed = completed;
        }
    }

    [SerializeField]
    private Text title = null;
    [SerializeField]
    private Text description = null;
    private float timer = 0;
    private Vector3 initPos = Vector3.zero;
    private List<QuestMessage> quests = new List<QuestMessage>();

    private void OnQuestAdded(QuestController sender, QuestController.QuestID quest)
    {
        quests.Add(new QuestMessage (quest.quest.name, quest.quest.description, false));
    }

    private void OnQuestCompleted(QuestController sender, QuestController.QuestID quest)
    {
        quests.Add(new QuestMessage(quest.quest.name, quest.quest.description, true));
    }

    private void Pop()
    {
        timer = 8f;
        if (quests[0].Completed)
        {
            title.text = "Objective Completed!";
            description.text = quests[0].Name;
        }
        else
        {
            title.text = quests[0].Name;
            description.text = quests[0].Description;
        }

        quests.RemoveAt(0);
    }

    void Start()
    {
        initPos = transform.localPosition;
        GameInstance.GameState.QuestController.OnQuestAdded += OnQuestAdded;
        GameInstance.GameState.QuestController.OnQuestCompleted += OnQuestCompleted;
        transform.localPosition = initPos - new Vector3(300, 0, 0);
    }

    private void OnDestroy()
    {
        GameInstance.GameState.QuestController.OnQuestAdded -= OnQuestAdded;
        GameInstance.GameState.QuestController.OnQuestCompleted -= OnQuestCompleted;
    }

    void Update()
    {
        if (quests.Count > 0 && timer == 0)
            Pop();

        timer = Mathf.Max(timer - Time.deltaTime, 0);

        if (timer > 2)
            transform.localPosition = Vector3.Lerp(transform.localPosition, initPos, Time.deltaTime * 3f);
        else
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(initPos.x - 300f, initPos.y, initPos.z), Time.deltaTime * 2f);
    }
}
