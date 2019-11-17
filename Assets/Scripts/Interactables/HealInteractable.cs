using System.Collections;
using UnityEngine;

public class HealInteractable : Interactable
{
    [SerializeField] private float baseHeal = 0.1f;
    private Coroutine healCoroutine = null;

    protected override void OnInteract(PlayerController controller)
    {
        if (healCoroutine != null)
            return;

        healCoroutine = StartCoroutine(ApplyHeal(controller));
    }

    private IEnumerator ApplyHeal(PlayerController controller)
    {
        controller.MovementSettings.CanControl = false;

        yield return new WaitForSecondsRealtime(1f);

        controller.ApplyHeal(new PointHeal(controller, baseHeal));

        controller.MovementSettings.CanControl = true;

        healCoroutine = null;
    }
}

