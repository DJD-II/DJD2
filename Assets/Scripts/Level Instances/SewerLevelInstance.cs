using UnityEngine;

public class SewerLevelInstance : LevelInstance, ISavable
{
    [System.Serializable]
    sealed public class SewerSavable : Savable
    {
        private bool LucasIsSafe { get; }

        public SewerSavable(SewerLevelInstance levelInstance)
            : base ("", "")
        {
            LucasIsSafe = !levelInstance.lucasIsInNeed;
        }

        public void Set(SewerLevelInstance levelInstance)
        {
            levelInstance.lucas.SetActive(!LucasIsSafe);
        }
    }

    private bool lucasIsInNeed = true;
    [SerializeField] private GameObject lucas = null;

    Savable ISavable.IO { get => new SewerSavable(this); }

    protected override void Start()
    {
        base.Start();

        GameInstance.GameState.QuestController.CompleteQuest("The homeless Entrace!");

        GameInstance.HUD.TalkUIController.OnAnswered += OnPlayerAnswered;

        if (lucasIsInNeed)
            GameInstance.HUD.OnTalkClose += OnTalkClose;
    }

    private void OnTalkClose(HUD sender)
    {
        if (lucasIsInNeed)
            return;

        GameInstance.HUD.OnTalkClose -= OnTalkClose;

        //standingSecurity2.GetComponent<MoverAI>().MoveTo(standingSecurity1.transform);

        //float y = standingSecurity1.transform.localRotation.eulerAngles.y;
        //standingSecurity2.GetComponent<TalkInteractable>().InitRotation =
        //    Quaternion.Euler(0f, y, 0f);
    }

    private void OnPlayerAnswered(TalkUIController sender, PlayerAnswer answer)
    {
        if (answer.ID != 101)
            return;

        lucasIsInNeed = false;
    }

    protected override void OnSave(SaveGame io)
    {
        base.OnSave(io);

        io.Override(this);
    }

    protected override void OnLoad(SaveGame io)
    {
        base.OnLoad(io);

        if (!(io.Find(x => x is SewerSavable) is SewerSavable savable))
            return;

        savable.Set(this);
    }
}
