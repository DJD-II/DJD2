using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System.IO;
using UnityEngine.UI;

public class TalkUIController : MonoBehaviour
{
    #region ---Inicialization---
    public GameObject answerButton;
    public GameObject conversationPanel;
    public GameObject answersPanel;
    public Text toTalkPanel;
    #endregion

    #region ---Properties---
    private int CurrentDialogue { get; set; }
    public TalkInteractable Interactable { get; set; }
    public PlayerController PlayerController { get; set; }
    #endregion

    private Coroutine slowLettersCoroutine;

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
    public void Initialize()
    {
        CurrentDialogue = 0;
        SwitchToConversation();
        toTalkPanel.text = Interactable.currentConvo.managerContents[0].nPCdialogue.text;
    }

    // Creates a button and puts the contents inside
    private void InstanciateAnswers()
    {
        while (answersPanel.transform.childCount > 0)
        {
            DestroyImmediate(answersPanel.transform.GetChild(0).gameObject);
        }

        foreach (PlayerAnswers i in Interactable.currentConvo.managerContents[CurrentDialogue].answers)
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

    // Checks the contents on the button clicked
    private void OnAnswer(AnswersButton sender)
    {
        ActiveAnswerCheck(sender.PAnswer);
        DialogueManager i = Interactable.currentConvo.managerContents.Find(x => x.nPCdialogue.id == sender.PAnswer.toID);

        if (i == null)
            Close();

        else
        {
            toTalkPanel.text = "";
            CurrentDialogue = Interactable.currentConvo.managerContents.IndexOf(i);
            slowLettersCoroutine = StartCoroutine(SlowLetters(i.nPCdialogue.text));
        }
    }

    private bool SkillCheck(PlayerAnswers other, AnswersButton button)
    {
        int playerIntellegence = 10;

        if (playerIntellegence < other.Intelligence)
        {
            button.label.text = "[Inteligence - " + other.Intelligence + "] - " + button.label.text;
            Color color = button.label.color;
            color.a = 0.5f;
            button.label.color = color;
            button.Disable();
            return false;
        }
        //if ("Implement_Necessaty skill checks")
        return true;
    }

    private void ActiveAnswerCheck(PlayerAnswers other)
    {
        if (other.exit)
        {
            Close();
            return;
        }
        if (other.itemsToGive != null)
        {
            foreach (Item i in other.itemsToGive)
                PlayerController.Inventory.Add(i);
        }

        if (other.questsToGive != null)
        {
            foreach (Quest q in other.questsToGive)
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
        Debug.LogError("Closing");
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
