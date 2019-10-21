using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class QuestUtility
{
    private readonly static Quest[] quests = Resources.LoadAll<Quest>("Quests");
    public static Quest Get(string name)
    {
        foreach (Quest q in quests)
            if (name.Equals(q.name.ToString()))
                return q;

        return null;
    }
}