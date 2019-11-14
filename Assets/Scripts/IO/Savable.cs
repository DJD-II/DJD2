[System.Serializable]
public abstract class Savable : object
{
    public string SceneName { get; protected set; }
    public string ID { get; protected set; }

    public Savable(string id, string sceneName)
    {
        ID = id;
        SceneName = sceneName;
    }
}