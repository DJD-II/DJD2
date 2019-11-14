using UnityEngine;

[System.Serializable]
sealed public class NewsID
{
    [SerializeField] private News[] news = null;
    [SerializeField] private Requesite.EventOperation[] events = null;

    public bool Fullfills()
    {
        bool fullfils = true;
        foreach (Requesite.EventOperation evnt in events)
            fullfils &= evnt.Calculate();

        return fullfils;
    }

    public News[] News { get => news; }
}
