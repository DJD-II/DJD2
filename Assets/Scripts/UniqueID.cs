using UnityEngine;

public class UniqueIdentifierAttribute : PropertyAttribute { }

sealed public class UniqueID : MonoBehaviour
{
    [UniqueIdentifierAttribute]
    public string uniqueId;
    public bool persistentAcrossLevels;

}