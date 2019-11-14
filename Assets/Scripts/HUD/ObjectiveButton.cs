using UnityEngine;
using UnityEngine.UI;

sealed public class ObjectiveButton : MonoBehaviour
{
    public delegate void EventHandler(ObjectiveButton sender);

    public event EventHandler OnClick,
                                OnHoverEnter,
                                OnHoverExit;

    [SerializeField]
    private Text objectiveLabel = null;
    [SerializeField]
    private Toggle objectiveCompletedToggle = null;
    public QuestID QuestID { get; private set; }

    public void Initialize(QuestID questId)
    {
        QuestID = questId;
        objectiveLabel.text = questId.quest.name;
        objectiveCompletedToggle.isOn = questId.completed;
    }

    public void OnHovered()
    {
        OnHoverEnter?.Invoke(this);
    }

    public void OnExitHover()
    {
        OnHoverExit?.Invoke(this);
    }

    public void Click()
    {
        OnClick?.Invoke(this);
    }
}
