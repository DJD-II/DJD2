using UnityEngine;

public abstract class Damage : InstigatedObject
{
    public Damage(Actor instigator, bool trueDamage)
        : base(instigator)
    {
        TrueDamage = trueDamage;
    }

    public virtual float Get(Actor actor)
    {
        return BaseDamage;
    }

    public virtual float BaseDamage { get; set; } = 10;
    public bool TrueDamage { get; }
}

public sealed class PointDamage : Damage
{
    public PointDamage(Actor instigator, bool trueDamage, float baseDamage)
        : base(instigator, trueDamage)
    {
        BaseDamage = baseDamage;
    }
}

public sealed class RadialDamage : Damage
{
    public RadialDamage(Actor instigator, Vector3 origin, float area, bool trueDamage, float baseDamage)
        : base(instigator, trueDamage)
    {
        Origin = origin;
        Area = area;
        BaseDamage = baseDamage;
    }

    public override float Get(Actor actor)
    {
        float distance = (Origin - actor.transform.position).magnitude;
        if (distance > Area)
            return 0;

        float influence = 1 - (distance / Area);

        if (actor is Controller character)
            return base.Get(actor) * influence * (100 / (100 + character.Armour.Value * 2));

        return base.Get(actor);
    }

    public Vector3 Origin { get; set; }
    public float Area { get; set; }
}