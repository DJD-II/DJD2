using System.Collections;
using UnityEngine;
using UnityEngine.UI;

sealed public class HackController : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private Text consoleTextTitle = null;
    [SerializeField] private Text consoleText = null;
    [SerializeField] private Text timerLabel = null;
    [SerializeField] private Text triesLabel = null;
    [SerializeField] private Text triesLogLabel = null;
    [SerializeField] private Text correctNumberLabel = null;
    [SerializeField] private AnimationCurve outputConsoleLogSpeed = null;
    [SerializeField] private string[] consoleTexts = new string[0];
    [Header("Hacking")]
    [SerializeField] private GameObject hackPanel = null;
    [SerializeField] private GameObject commandPanel = null;
    [SerializeField] private HackButton[] buttons = new HackButton[0];
    [SerializeField] private float showLetterDelay = 0.01f;
    [SerializeField] private GameObject commandButton = null;
    [SerializeField] private Text hackPointText = null;
    [SerializeField] private Button forcePasswordButton = null;
    [SerializeField] private Transform commandsContent = null;
    [Header("SFXs")]
    [SerializeField] private AudioSource turnOnSFX = null;
    [SerializeField] private AudioSource turnOffSFX = null;
    [SerializeField] private AudioSource LoopSFX = null;
    private float timer = 60;
    private Coroutine hackPointCoroutine = null;

    public HackInteractable Interactable { get; private set; }
    public PlayerController PlayerController { get; private set; }

    public void Initialize(HackInteractable interactable, 
                           PlayerController controller)
    {
        Interactable = interactable;
        PlayerController = controller;
        consoleText.text = "";

        forcePasswordButton.interactable = true;
        foreach (HackButton b in buttons)
            b.ResetNumber();

        triesLabel.text = "6";
        consoleText.gameObject.SetActive(true);
        consoleTextTitle.gameObject.SetActive(true);
        commandPanel.SetActive(false);
        hackPanel.SetActive(false);
        triesLogLabel.text = "";
        timer = 60;
        correctNumberLabel.text = GenerateRandomPassword();

        StartCoroutine(LogConsoleText());

        turnOnSFX.Play();
    }

    private IEnumerator LogConsoleText()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        foreach (string s in consoleTexts)
        {
            consoleText.text += "\n" + s;
            yield return new WaitForSecondsRealtime(
                outputConsoleLogSpeed.Evaluate(Time.unscaledTime));
        }

        yield return new WaitForSecondsRealtime(2f);

        consoleText.gameObject.SetActive(false);
        consoleTextTitle.gameObject.SetActive(false);

        if (Interactable.Locked)
            hackPanel.SetActive(true);
        else
            SwitchToCommands();
    }

    /// <summary>
    /// Generates a password with unique numbers.
    /// </summary>
    /// <returns>The generated password as string.</returns>
    private string GenerateRandomPassword ()
    {
        string tempPassword = "";
        while (tempPassword.Length < 4)
        {
            string randomChar = Random.Range(0, 9).ToString();
            if (tempPassword.Contains(randomChar))
                continue;

            tempPassword += randomChar;
        }
        return tempPassword;
    }

    private void Update()
    {
        if (!turnOnSFX.isPlaying && !LoopSFX.isPlaying)
            LoopSFX.Play();

        timerLabel.text = timer.ToString("f0") + " s";

        if (!Interactable.Locked)
            return;

        timer = Mathf.Max(timer - Time.unscaledDeltaTime, 0);

        if (timer <= 0)
            Close();
    }

    public void Lockdown()
    {
        string myPass = "";
        int i = 0;
        foreach (HackButton button in buttons)
        {
            string s = button.Number.ToString();
            myPass += s;
            if (correctNumberLabel.text[i] == s[0])
                button.SetColor(Color.green);
            else if (correctNumberLabel.text.Contains(s.ToString()))
                button.SetColor(Color.yellow);
            else
                button.SetColor(Color.red);
            i++;
        }

        if (myPass == correctNumberLabel.text)
            StartCoroutine(Unlock());
    }

    public IEnumerator Unlock()
    {
        forcePasswordButton.interactable = false;
        foreach (HackButton button in buttons)
            button.Lock();

        Interactable.Unlock(PlayerController);
        yield return new WaitForSecondsRealtime(1f);
        hackPanel.SetActive(false);
        
        SwitchToCommands();
    }

    private void SwitchToCommands()
    {
        commandPanel.SetActive(true);

        ShowHackPoint(0);
    }

    private void ShowHackPoint(int index)
    {
        if (Interactable.Hack == null || 
            index < 0 ||
            index >= Interactable.Hack.HackPoints.Count)
        {
            Close();
            return;
        }

        Hack.Point hackPoint = Interactable.Hack.HackPoints[index];

        if (hackPointCoroutine != null)
        {
            StopCoroutine(hackPointCoroutine);
            hackPointCoroutine = null;
        }

        hackPointCoroutine = StartCoroutine(ShowLetters(hackPoint.Text));

        while (commandsContent.childCount > 0)
            DestroyImmediate(commandsContent.GetChild(0).gameObject);

        foreach (Hack.Point.Command command in hackPoint.Commands)
        {
            if (!command.Fullfills(PlayerController))
                continue;

            GameObject go = Instantiate(commandButton, commandsContent);
            HackCommandButton hcb = go.GetComponent<HackCommandButton>();
            hcb.Initialize(command);

            hcb.OnClicked += OnHackCommandClicked;
        }
    }

    private void OnHackCommandClicked(HackCommandButton sender)
    {
        sender.Command.Process(PlayerController);

        ShowHackPoint(sender.Command.ToID);
    }

    private IEnumerator ShowLetters (string text)
    {
        hackPointText.text = "";

        text = text.Replace("<br>", "\n");
        for (int i = 0; i < text.Length; i++)
        {
            hackPointText.text += text[i];

            yield return new WaitForSecondsRealtime(showLetterDelay);
        }

        hackPointCoroutine = null;
    }

    public void Close()
    {
        turnOffSFX.Play();
        LoopSFX.Stop();
        GameInstance.HUD.EnableHacking(false);
    }
}
