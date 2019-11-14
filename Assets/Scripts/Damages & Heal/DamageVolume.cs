using UnityEngine;

[RequireComponent(typeof(Collider))]
sealed public class DamageVolume : Actor
{
    [Header("Settings")]
    [SerializeField] private bool isRadial = false;
    [SerializeField] private bool trueDamage = false;
    [SerializeField] private float baseDamage = 0.4f;
    private Damage damage = null;
    private Collider volumeCollider = null;

    private void Awake()
    {
        volumeCollider = GetComponent<Collider>();
        volumeCollider.isTrigger = true;

        if (isRadial)
            damage = new RadialDamage(this,
                                      transform.position,
                                      volumeCollider.bounds.size.magnitude,
                                      trueDamage,
                                      baseDamage);
        else
            damage = new PointDamage(this, trueDamage, baseDamage);
    }

    private void Update()
    {
        damage.BaseDamage = baseDamage * Time.deltaTime;

        if (!isRadial)
            return;

        ((RadialDamage)damage).Origin = transform.position;
        ((RadialDamage)damage).Area = volumeCollider.bounds.size.magnitude;
    }

    private void OnTriggerStay(Collider other)
    {
        Actor actor = other.GetComponent<Actor>();
        if (actor == null)
            return;

        actor.ApplyDamage(damage);
    }

    protected override bool OnApplyDamage(Damage damage)
    {
        return false;
    }

    protected override bool OnApplyHeal(Heal heal)
    {
        return false;
    }
}
