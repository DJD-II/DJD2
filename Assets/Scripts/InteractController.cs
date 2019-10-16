using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InteractController
{
    [SerializeField]
    private Transform origin = null;
    [Range (0.01f, 1f)]
    [SerializeField]
    private float interactTime = 0.15f;
    [SerializeField]
    [Range(1f, 500f)]
    private float interactDistance = 100f;
    [SerializeField]
    private LayerMask ignoreLayers = 0;
    private Interactable interactable = null;

    public float InteractTime { get { return interactTime; } }

    public void Update(PlayerController controller)
    {
        if (interactable == null)
            return;

        if (Input.GetKeyDown(KeyCode.E))
            interactable.Interact(controller);
    }

    public void UpdateInteraction()
    {
        if (GameInstance.GameState.Paused)
            return;

        GameInstance.HUD.EnableInteractMessage(false, null);
        this.interactable = null;

        RaycastHit[] hits = Physics.RaycastAll(new Ray(origin.position, origin.forward), interactDistance, ~ignoreLayers);
        List<RaycastHit> hitList = new List<RaycastHit>(hits);
        hitList.Sort((x, y) => Vector3.Distance(x.point, origin.position).CompareTo(Vector3.Distance(y.point, origin.position)));
        foreach (RaycastHit hit in hitList)
        {
            Interactable interactable = hit.collider.gameObject.GetComponent<Interactable>();
            if (interactable != null)
            {
                GameInstance.HUD.EnableInteractMessage(true, interactable);
                this.interactable = interactable;
                break;
            }
        }
    }
}
