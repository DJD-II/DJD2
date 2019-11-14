using UnityEngine;

abstract public class Controller : Actor
{
    public event EventHandler   OnDeath,
                                OnHealed,
                                OnDamaged,
                                OnRevived;

    [Header("Controller")]
    [Tooltip("Defines the controller health.")]
    [SerializeField] protected ScaledValue hp = new ScaledValue(1, 100);
    [Tooltip("Defines the controller armour.")]
    [SerializeField] protected ScaledValue armour = new ScaledValue(1, 0);
    private UniqueID uniqueID = null;

    /// <summary>
    /// The players life.
    /// </summary>
    public ref ScaledValue Hp { get { return ref hp; } }
    public ref ScaledValue Armour { get { return ref armour; } }
    public string UniqueID
    {
        get
        {
            if (uniqueID == null)
                return "";

            return uniqueID.Id;
        }
    }
    public bool PersistentAcrossLevels
    {
        get
        {
            if (uniqueID == null)
                return false;

            return uniqueID.PersistentAcrossLevels;
        }
    }

    protected virtual void Awake()
    {
        uniqueID = GetComponent<UniqueID>();

        GameInstance.OnLoad += OnLoad;
    }

    protected virtual void Start()
    {
        GameInstance.OnSave += OnSave;
    }

    protected virtual void OnDestroy()
    {
        GameInstance.OnLoad -= OnLoad;
        GameInstance.OnSave -= OnSave;
    }

    /// <summary>
    /// This method is called when a saved game is loading.
    /// </summary>
    /// <param name="io">The object that holds all saved objects.</param>
    protected virtual void OnLoad (SaveGame io)
    {

    }

    /// <summary>
    /// This method is called when the game is about to be saved.
    /// </summary>
    /// <param name="io">The object that holds all saved objects.</param>
    protected virtual void OnSave(SaveGame io)
    {

    }

    /// <summary>
    /// This method is called when damage is applied to this controller.
    /// </summary>
    /// <param name="damage">The type of damage to be applied.</param>
    /// <returns>Returns true if damage was applied.</returns>
    protected override bool OnApplyDamage(Damage damage)
    {
        bool isAlive = !Hp.IsEmpty;

        if (!isAlive)
            return false;

        if (damage.TrueDamage)
            hp.SubtractLimit(damage.Get(this));
        else
            hp.Subtract(damage.Get(this));

        if (isAlive && Hp.IsEmpty)
            OnDeath?.Invoke(this);
        else
            OnDamaged?.Invoke(this);

        return true;
    }

    /// <summary>
    /// This method is called when heal is applied to this controller.
    /// </summary>
    /// <param name="heal">The type of heal to be applied.</param>
    /// <returns>Returns true if heal was applied.</returns>
    protected override bool OnApplyHeal(Heal heal)
    {
        bool isAlive = !Hp.IsEmpty;

        if (isAlive)
        {
            if (heal.TrueHeal)
                hp.AddLimit(heal.Get(this));
            else
                hp.Add(heal.Get(this));

            OnHealed?.Invoke(this);
            return true;
        }
        else if (CanBeRevived)
        {
            hp.Add(heal.Get(this));
            OnRevived?.Invoke(this);
            return true;
        }

        return false;
    }
}