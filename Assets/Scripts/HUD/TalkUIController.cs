using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

sealed public class TalkUIController : MonoBehaviour
{
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

    private Coroutine slowLettersCoroutine;

    public void Initialize()
    {
        CurrentConversation = Interactable.Conversation;
        CurrentManager = CurrentConversation.dialogues[0];
        SwitchToConversation();
        slowLettersCoroutine = StartCoroutine(SlowLetters(CurrentManager.Dialogue.Text));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (slowLettersCoroutine != null)
                StopCoroutine(slowLettersCoroutine);
            SwitchToAnswers();
            InstantiateAnswers();
        }
    }

    // Creates a button and puts the contents inside
    private void InstantiateAnswers()
    {
        if (CurrentManager.Dialogue.ToAnswerID < 0 || CurrentManager.Dialogue.ToAnswerID >= CurrentConversation.Answers.Count)
        {
            Close();
            return;
        }

        while (answersPanel.transform.childCount > 0)
            DestroyImmediate(answersPanel.transform.GetChild(0).gameObject);

        foreach (PlayerAnswer i in CurrentConversation.Answers[CurrentManager.Dialogue.ToAnswerID].Answers)
        {
            GameObject go = Instantiate(answerButton, answersPanel.transform);
            AnswersButton button = go.GetComponent<AnswersButton>();
            button.Initialize(i);
            button.OnClick += OnAnswer;
        }
    }

    // Checks the contents on the button clicked
    private void OnAnswer(AnswersButton sender)
    {
        if (sender.pAnswer.toDialogueID < 0 || sender.pAnswer.toDialogueID >= CurrentConversation.dialogues.Count)
        {
            Close();
            return;
        }

        DialogueManager i = CurrentConversation.dialogues[sender.pAnswer.toDialogueID];

        if (i == null)
            Close();
        else
            slowLettersCoroutine = StartCoroutine(SlowLetters(i.Dialogue.Text));
    }

    private IEnumerator SlowLetters(string other)
    { 
        SwitchToConversation();

        for (int i = 0; i < other.Length; i++)
        {
            toTalkPanel.text += (other[i]);
            yield return new WaitForSecondsRealtime(0.15f);
        }
    }

    private void Close()
    {
        if (slowLettersCoroutine != null)
            StopCoroutine(slowLettersCoroutine);
        GameInstance.GameState.Paused = false;
        gameObject.SetActive(false);
    }

    public void SwitchToAnswers()
    {
        if (conversationPanel != null)
            conversationPanel.SetActive(false);

        if (answersPanel != null)
            answersPanel.SetActive(true);
    }

    public void SwitchToConversation()
    {
        toTalkPanel.text = "";

        if (conversationPanel != null)
            conversationPanel.SetActive(true);

        if (answersPanel != null)
            answersPanel.SetActive(false);
    }
}
