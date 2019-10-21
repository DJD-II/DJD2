using System.Collections;
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

    private int CurrentDialogue { get; set; }
    public TalkInteractable Interactable { get; set; }
    public PlayerController PlayerController { get; set; }

    private Coroutine slowLettersCoroutine;
    public void Initialize()
    {
        CurrentDialogue = 0;
        SwitchToConversation();
        toTalkPanel.text = Interactable.Conversation.managerContents[0].nPCdialogue.text;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (slowLettersCoroutine != null)
                StopCoroutine(slowLettersCoroutine);
            SwitchToAnswers();
            InstanciateAnswers();
        }
    }

    // Creates a button and puts the contents inside
    private void InstanciateAnswers()
    {
        while (answersPanel.transform.childCount > 0)
        {
            DestroyImmediate(answersPanel.transform.GetChild(0).gameObject);
        }

        foreach (PlayerAnswer i in Interactable.Conversation.managerContents[CurrentDialogue].answers)
        {
            GameObject go = Instantiate(answerButton, answersPanel.transform);
            AnswersButton button = go.GetComponent<AnswersButton>();
            button.Initialize(i);

            if (SkillCheck(i, button))
            {
                button.OnClick += OnAnswer;
            }

        }
    }
    private bool SkillCheck(PlayerAnswer other, AnswersButton button)
    {
        int playerIntellegence = 10;

        if (playerIntellegence < other.Intelligence)
        {
            button.Label.text = "[Inteligence - " + other.Intelligence + "] - " + button.Label.text;
            Color color = button.Label.color;
            color.a = 0.5f;
            button.Label.color = color;
            button.Disable();
            return false;
        }
        //if ("Implement_Necessaty skill checks")
        return true;
    }

    // Checks the contents on the button clicked
    private void OnAnswer(AnswersButton sender)
    {
        ActiveAnswerCheck(sender.PAnswer);

        DialogueManager i = Interactable.Conversation.managerContents.Find(x => x.nPCdialogue.id == sender.PAnswer.toID);


        if (i == null)
            Close();

        else
        {
            toTalkPanel.text = "";
            CurrentDialogue = Interactable.Conversation.managerContents.IndexOf(i);
            slowLettersCoroutine = StartCoroutine(SlowLetters(i.nPCdialogue.text));
        }
    }
    private void ActiveAnswerCheck(PlayerAnswer other)
    {
        foreach (PlayerAnswer.Command c in other.command)
            switch (c)
            {
                case PlayerAnswer.Command.Quit:
                    Close();
                    break;

                case PlayerAnswer.Command.GiveItem:
                    foreach (Item i in other.itemsToGive)
                    {
                        PlayerController.Inventory.Add(i);
                    };
                    break;

                case PlayerAnswer.Command.GiveQuest:
                    foreach (Quest q in other.questsToGive)
                    {
                        GameInstance.GameState.QuestController.Add(other.questsToGive[0]);
                    }
                    break;
                case PlayerAnswer.Command.AddMoney:
                    Debug.Log("IMPLEMENT_MONEY");
                    break;
                case PlayerAnswer.Command.SubtractMoney:
                    Debug.Log("IMPLEMENT_MONEY");
                    break;
            }

        if (other.questsToGive != null)
        {
            foreach (Quest q in other.questsToGive.ToArray())
                GameInstance.GameState.QuestController.Add(other.questsToGive[0]);
        }

        if (other.cost != 0)
            Debug.Log("IMPLEMENT_MONEY");

    }
    private IEnumerator SlowLetters(string other)
    {
        SwitchToConversation();

        for (int i = 0; i < other.Length; i++)
        {
            toTalkPanel.text += (other[i]);
            yield return new WaitForSecondsRealtime(0.07f);
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
        if (conversationPanel != null)
            conversationPanel.SetActive(true);

        if (answersPanel != null)
            answersPanel.SetActive(false);
    }
}
