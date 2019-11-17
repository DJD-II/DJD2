using UnityEngine;
using UnityEngine.SceneManagement;

public class StripClubLevelInstance : LevelInstance, ISavable
{
    [System.Serializable]
    public class StripSavable : Savable
    {
        public bool locked;

        public StripSavable(StripClubLevelInstance levelInstance)
            : base("", SceneManager.GetActiveScene().name)
        {
            locked = !levelInstance.foundOut;
        }

        public void Set(StripClubLevelInstance levelInstance)
        {
            if (levelInstance.foundOut = !locked)
            {
                levelInstance.standingSecurity1.SetActive(true);
                levelInstance.standingSecurity2.SetActive(false);
            }
        }
    }

    [SerializeField] private GameObject standingSecurity1 = null;
    [SerializeField] private GameObject standingSecurity2 = null;
    private bool foundOut = false;

    protected override void Start()
    {
        base.Start();

        GameInstance.HUD.TalkUIController.OnAnswered += OnPlayerAnswered;

        if (!foundOut)
            GameInstance.HUD.OnTalkClose += OnTalkClose;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        GameInstance.HUD.TalkUIController.OnAnswered -= OnPlayerAnswered;
        GameInstance.HUD.OnTalkClose -= OnTalkClose;
    }

    protected override void OnSave(SaveGame io)
    {
        base.OnSave(io);

        io.Override(this);
    }

    protected override void OnLoad(SaveGame io)
    {
        base.OnLoad(io);

        if (!(io.Find(x => x is StripSavable) is StripSavable savable))
            return;

        savable.Set(this);
    }

    private void OnTalkClose(HUD sender)
    {
        if (!foundOut)
            return;

        GameInstance.HUD.OnTalkClose -= OnTalkClose;

        standingSecurity2.GetComponent<MoverAI>().MoveTo(standingSecurity1.transform);

        float y = standingSecurity1.transform.localRotation.eulerAngles.y; 
        standingSecurity2.GetComponent<TalkInteractable>().InitRotation = 
            Quaternion.Euler(0f, y, 0f);
    }

    private void OnPlayerAnswered(TalkUIController sender, PlayerAnswer answer)
    {
        if (answer.ID != 101)
            return;

        foundOut = true;
    }

    Savable ISavable.IO { get => new StripSavable(this); }
}
