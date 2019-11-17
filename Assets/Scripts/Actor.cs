using UnityEngine;

public abstract class Actor : MonoBehaviour
{
    public delegate void EventHandler(Actor sender);

    [Header("Actor")]
    [Tooltip("Defines if the player can be damaged.")]
    [SerializeField] private bool canBeDamaged = true;
    [Tooltip("Defines if the player can be healed.")]
    [SerializeField] private bool canBeHealed = true;
    [Tooltip("Defines if the player can be revived.")]
    [SerializeField] private bool canBeRevived = true;

    public bool CanBeDamaged
    {
        get => canBeDamaged;

        set => canBeDamaged = value;
    }
    public bool CanBeHealed
    {
        get => canBeHealed;

        set => canBeHealed = value;
    }
    public bool CanBeRevived
    {
        get => canBeRevived;

        set => canBeRevived = value;
    }

    /// <summary>
    /// Call this method to Apply damage to this actor.
    /// Damage is only applied if this actor can be damaged flag
    /// is on (true).
    /// </summary>
    /// <param name="damage">The type of damage to be applied.</param>
    /// <returns>Returns true if the damage was applied.</returns>
    public bool ApplyDamage(Damage damage)
    {
        // If the actor can't be damaged then return false (The actor was not damaged).
        // Otherwise Apply damage through OnApplyDamage and return
        // if the actor was indeed damaged.
        if (canBeDamaged)
            return OnApplyDamage(damage);

        return false;
    }

    /// <summary>
    /// This method is called when damage is applied to this Actor.
    /// This method should be overriden by inherited objects so that,
    /// they can define what will be like when damage is applied.
    /// </summary>
    /// <param name="damage">The type of damage to be applied.</param>
    /// <returns>Returns true if damage was applied.</returns>
    protected abstract bool OnApplyDamage(Damage damage);

    /// <summary>
    /// Call this method to Apply heal to this actor.
    /// Heal is only applied if this actor can be healed flag
    /// is on (true).
    /// </summary>
    /// <param name="heal">The type of heal to be applied.</param>
    /// <returns>Returns true if the heal was applied.</returns>
    public bool ApplyHeal(Heal heal)
    {
        // If the actor can't be healed then return false (The actor was not healed).
        // Otherwise Apply heal through OnApplyHeal and return
        // if the actor was indeed healed.
        if (canBeHealed)
            return OnApplyHeal(heal);

        return false;
    }

    /// <summary>
    /// This method is called when heal is applied to this Actor.
    /// This method should be overriden by inherited objects so that,
    /// they can define what will be like when heal is applied.
    /// </summary>
    /// <param name="heal">The type of heal to be applied.</param>
    /// <returns>Returns true if heal was applied.</returns>
    protected abstract bool OnApplyHeal(Heal heal);
}
