using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

sealed public class QuestMessageController : MonoBehaviour
{
    [SerializeField]
    private Text title = null;
    [SerializeField]
    private Text description = null;
    private float timer = 0;
    private Vector3 initPos = Vector3.zero;
    private List<QuestController.QuestID> quests = new List<QuestController.QuestID>();

    private void OnQuestAdded(QuestController sender, QuestController.QuestID quest)
    {
        quests.Add(quest);
    }

    private void OnQuestCompleted(QuestController sender, QuestController.QuestID quest)
    {
        timer = 0;
        transform.localPosition = initPos - new Vector3(300, 0, 0);
        quests.Add(quest);
    }

    private void Pop()
    {
        timer = 8f;
        if (quests[0].completed)
        {
            title.text = "Objective Completed!";
            description.text = quests[0].quest.name;
        }
        else
        {
            title.text = quests[0].quest.name;
            description.text = quests[0].quest.description;
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

        if (timer > 0)
            transform.localPosition = Vector3.Lerp(transform.localPosition, initPos, Time.deltaTime * 3f);
        else
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(initPos.x - 300f, initPos.y, initPos.z), Time.deltaTime * 2f);
    }
}
