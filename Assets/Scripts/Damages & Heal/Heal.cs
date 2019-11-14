using UnityEngine;


public abstract class Heal : InstigatedObject
{
    private bool trueHeal = false;
    public Heal(Actor instigator, bool trueHeal)
        : base(instigator)
    {
        this.trueHeal = trueHeal;
    }

    public virtual float Get(Actor actor)
    {
        return BaseHeal;
    }

    public virtual float BaseHeal { get; set; } = 10;
    public bool TrueHeal { get => trueHeal; }
}

public sealed class PointHeal : Heal
{
    public PointHeal(Actor instigator, float baseHeal, bool trueHeal = false)
        : base(instigator, trueHeal)
    {
        BaseHeal = baseHeal;
    }
}

public sealed class RadialHeal : Heal
{
    public RadialHeal(Actor instigator, Vector3 origin, float area, bool trueHeal = false)
        : base(instigator, trueHeal)
    {
        Origin = origin;
        Area = area;
    }

    public override float Get(Actor actor)
    {
        float distance = (Origin - actor.transform.position).magnitude;
        if (distance > Area)
            return 0;

        float influence = 1 - (distance / Area);
        return base.Get(actor) * influence;
    }

    public Vector3 Origin { get; }
    public float Area { get; }
}