using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

sealed public class QuestMessageController : MonoBehaviour, ISavable
{
    [System.Serializable]
    sealed public class QuestMessageSavable : Savable
    {
        List<QuestMessage> Messages { get; }

        public QuestMessageSavable(QuestMessageController controller)
            : base ("", "")
        {
            Messages = new List<QuestMessage>();
            if (controller.currentMessage != null)
                Messages.Add(controller.currentMessage);
            Messages.AddRange(controller.quests.ToArray());
        }

        public void Set(QuestMessageController controller)
        {
            controller.quests = Messages;
        }
    }

    [System.Serializable]
    private class QuestMessage
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public bool Completed { get; private set; }

        public QuestMessage(string name, string description, bool completed)
        {
            Name = name;
            Description = description;
            Completed = completed;
        }
    }

    [SerializeField] private Text title = null;
    [SerializeField] private Text description = null;
    [SerializeField] private float popTime = 8f;
    private QuestMessage currentMessage = null;
    private float timer = 0;
    private Vector3 initPos = Vector3.zero;
    private List<QuestMessage> quests = new List<QuestMessage>();

    Savable ISavable.IO { get => new QuestMessageSavable(this); }

    private void Awake()
    {
        GameInstance.OnLoad += OnLoad;
    }

    private void Start()
    {
        GameInstance.OnSave += OnSave;

        initPos = transform.localPosition;
        GameInstance.GameState.QuestController.OnQuestAdded += OnQuestAdded;
        GameInstance.GameState.QuestController.OnQuestCompleted += OnQuestCompleted;
        transform.localPosition = initPos - new Vector3(300, 0, 0);
    }

    private void Update()
    {
        if (quests.Count > 0 && timer == 0)
            Pop();
        else if (quests.Count == 0 && timer == 0)
            currentMessage = null;

        timer = Mathf.Max(timer - Time.deltaTime, 0);

        if (timer > 2)
            transform.localPosition = Vector3.Lerp(
                transform.localPosition, 
                initPos, 
                Time.deltaTime * 3f);
        else
            transform.localPosition = Vector3.Lerp(
                transform.localPosition, 
                new Vector3(initPos.x - 300f, initPos.y, initPos.z), 
                Time.deltaTime * 2f);
    }

    private void OnDestroy()
    {
        GameInstance.GameState.QuestController.OnQuestAdded -= OnQuestAdded;
        GameInstance.GameState.QuestController.OnQuestCompleted -= OnQuestCompleted;
    }

    private void OnQuestAdded(QuestController sender, QuestID quest)
    {
        quests.Add(new QuestMessage(quest.quest.name, quest.quest.description, false));
    }

    private void OnQuestCompleted(QuestController sender, QuestID quest)
    {
        quests.Add(new QuestMessage(quest.quest.name, quest.quest.description, true));
    }

    private void Pop()
    {
        timer = popTime;
        currentMessage = quests[0];

        if (currentMessage.Completed)
        {
            title.text = "Objective Completed!";
            description.text = currentMessage.Name;
        }
        else
        {
            title.text = currentMessage.Name;
            description.text = currentMessage.Description;
        }

        quests.RemoveAt(0);
    }

    private void OnSave(SaveGame io)
    {
        io.Override(this);
    }

    private void OnLoad(SaveGame io)
    {
        if (!(io.Find(x => x is QuestMessageSavable) is QuestMessageSavable savable))
            return;

        savable.Set(this);
    }
}
