using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

sealed public class TalkUIController : MonoBehaviour
{
    public delegate void EventHandler(TalkUIController sender, PlayerAnswer answer);

    public event EventHandler OnAnswered;

    [SerializeField]
    private float letterSpeed = 0.08f;
    [SerializeField]
    private GameObject conversationPanel = null;
    [SerializeField]
    private GameObject answersPanel = null;
    [SerializeField]
    private Text toTalkPanel = null;
    [SerializeField]
    private GameObject answerButton = null;

    private Conversation CurrentConversation { get; set; }
    private DialogueManager CurrentManager { get; set; }
    public TalkInteractable Interactable { get; set; }
    public PlayerController PlayerController { get; set; }
    public PlayerAnswer[] Answers { get; private set; }

    private Coroutine slowLettersCoroutine;

    public void Initialize()
    {
        CurrentConversation = Interactable.Conversation;
        CurrentManager = CurrentConversation.dialogues[0];
       
        Animator animator = Interactable.GetComponent<Animator>();
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        animator.runtimeAnimatorController = Interactable.Controller;

        SwitchToConversation();
    }

    private void Update()
    {
        Vector3 dir = PlayerController.transform.position - Interactable.transform.position;
        dir.y = 0;
        Interactable.transform.rotation = Quaternion.Lerp(Interactable.transform.rotation, Quaternion.LookRotation(dir), Time.unscaledDeltaTime * 3f);

        if (Input.anyKeyDown && toTalkPanel.gameObject.activeInHierarchy)
        {
            if (slowLettersCoroutine != null)
            {
                StopCoroutine(slowLettersCoroutine);
                toTalkPanel.text = CurrentManager.Dialogue.Text;
                slowLettersCoroutine = null;
            }
            else
            {
                switch(CurrentManager.Dialogue.SwitchTo)
                {
                    case SwitchType.Answers:
                        if (CurrentManager.Dialogue.ToID < 0 || CurrentManager.Dialogue.ToID >= CurrentConversation.Answers.Count)
                        {
                            Close();
                            return;
                        }
                        Answers = CurrentConversation.Answers[CurrentManager.Dialogue.ToID].Answers;
                        SwitchToAnswers();
                        break;

                    case SwitchType.Dialogue:

                        if (CurrentManager.Dialogue.ToID < 0 || CurrentManager.Dialogue.ToID >= CurrentConversation.dialogues.Count)
                        {
                            Close();
                            return;
                        }

                        CurrentManager = CurrentConversation.dialogues[CurrentManager.Dialogue.ToID];
                        SwitchToConversation();
                        break;
                }
            }
        }
    }

    // Creates a button and puts the contents inside
    private void InstantiateAnswers()
    {
        while (answersPanel.transform.childCount > 0)
            DestroyImmediate(answersPanel.transform.GetChild(0).gameObject);

        foreach (PlayerAnswer i in Answers)
        {
            if (!i.Requesites.Fullfills(PlayerController))
                continue;

            GameObject go = Instantiate(answerButton, answersPanel.transform);
            AnswersButton button = go.GetComponent<AnswersButton>();
            button.Initialize(i);
            button.OnClick += OnAnswer;
        }
    }

    // Checks the contents on the button clicked
    private void OnAnswer(AnswersButton sender)
    {
        OnAnswered?.Invoke(this, sender.pAnswer);

        foreach (Quest q in sender.pAnswer.QuestsToComplete)
        {
            QuestController.QuestID id = GameInstance.GameState.QuestController.Quests.Find(x => x.quest.name.ToLower().Equals(q.name.ToLower()));
            if (id != null)
                id.Complete();
        }

        foreach (Quest q in sender.pAnswer.QuestsToEarn)
            GameInstance.GameState.QuestController.Add(q);

        foreach (Item i in sender.pAnswer.ItemsToGive)
            PlayerController.Inventory.Remove(i.name);

        foreach (Item i in sender.pAnswer.ItemsToEarn)
            PlayerController.Inventory.Add(i);

        switch (sender.pAnswer.SwitchTo)
        {
            case SwitchType.Dialogue:
                if (sender.pAnswer.ToID < 0 || sender.pAnswer.ToID >= CurrentConversation.dialogues.Count)
                {
                    Close();
                    return;
                }

                CurrentManager = CurrentConversation.dialogues[sender.pAnswer.ToID];
                if (CurrentManager == null)
                    Close();
                else
                    SwitchToConversation();
                break;

            case SwitchType.Answers:

                if (sender.pAnswer.ToID < 0 || sender.pAnswer.ToID >= CurrentConversation.Answers.Count)
                {
                    Close();
                    return;
                }

                Answers = CurrentConversation.Answers[sender.pAnswer.ToID].Answers;
                SwitchToAnswers();

                break;
        }

       
    }

    private IEnumerator SlowLetters(string other)
    { 
        for (int i = 0; i < other.Length; i++)
        {
            toTalkPanel.text += (other[i]);
            yield return new WaitForSecondsRealtime(letterSpeed);
        }

        slowLettersCoroutine = null;
    }

    private void Close()
    {
        Interactable.IsTalking = false;
        Animator animator = Interactable.GetComponent<Animator>();
        animator.updateMode = AnimatorUpdateMode.Normal;
        animator.runtimeAnimatorController = Interactable.InitController;
        if (slowLettersCoroutine != null)
            StopCoroutine(slowLettersCoroutine);
        GameInstance.GameState.Paused = false;
        gameObject.SetActive(false);
    }

    public void SwitchToAnswers()
    {
        Interactable.GetComponent<Animator>().SetBool("Talking", false);
        InstantiateAnswers();
        if (conversationPanel != null)
            conversationPanel.SetActive(false);

        if (answersPanel != null)
            answersPanel.SetActive(true);
    }

    public void SwitchToConversation()
    {
        Interactable.GetComponent<Animator>().SetBool("Talking", true);
        toTalkPanel.text = "";
        slowLettersCoroutine = StartCoroutine(SlowLetters(CurrentManager.Dialogue.Text));

        if (conversationPanel != null)
            conversationPanel.SetActive(true);

        if (answersPanel != null)
            answersPanel.SetActive(false);
    }
}
