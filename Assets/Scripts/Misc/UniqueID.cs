using UnityEngine;

public class UniqueIdentifierAttribute : PropertyAttribute
{
}

[DisallowMultipleComponent]
sealed public class UniqueID : MonoBehaviour
{
    [UniqueIdentifierAttribute]
    [SerializeField] private string uniqueId;
    [SerializeField] private bool persistentAcrossLevels = false;

    public string Id { get => uniqueId; set => uniqueId = value; }
    public bool PersistentAcrossLevels { get => persistentAcrossLevels; }
}