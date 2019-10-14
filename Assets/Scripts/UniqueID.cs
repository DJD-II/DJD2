using UnityEngine;

public class UniqueIdentifierAttribute : PropertyAttribute { }


public class UniqueID : MonoBehaviour
{
    [UniqueIdentifierAttribute]
    public string uniqueId;
    public bool persistentAcrossLevels;

}