using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InteractController
{
    [SerializeField] private Transform origin = null;
    [Range(0.01f, 1f)]
    [SerializeField] private float interactTime = 0.15f;
    [Range(1f, 500f)]
    [SerializeField] private float interactDistance = 100f;
    [SerializeField] private LayerMask ignoreLayers = 0;
    
    public Interactable Interactable { get; private set; }
    public float InteractTime { get => interactTime; }

    /// <summary>
    /// Called when the game pause state changes.
    /// </summary>
    /// <param name="sender">The Game State.</param>
    public void OnPausedChanged(GameState sender)
    {
        if (!sender.Paused)
            return;

        GetInteractable(false);
    }

    /// <summary>
    /// Updates this controller. It checks for user input
    /// And Interacts with an interactable object.
    /// </summary>
    /// <param name="controller"></param>
    public void Update(PlayerController controller)
    {
        if (Input.GetKeyDown(KeyCode.E))
            Interact(controller);
    }

    /// <summary>
    /// Forces an interaction between the player controller and
    /// the interactable object.
    /// </summary>
    /// <param name="controller">The Player Controller.</param>
    public void Interact (PlayerController controller)
    {
        if (Interactable == null)
            return;

        Interactable.Interact(controller);
    }

    /// <summary>
    /// Updates the object that can be interacted with.
    /// </summary>
    /// <param name="canControl">If the player is in control.</param>
    public void UpdateInteraction(bool canControl)
    {
        GameInstance.HUD.EnableInteractMessage(false);
        Interactable = null;

        if (GameInstance.GameState.Paused || !canControl)
            return;

        GetInteractable();
    }

    /// <summary>
    /// Gets an interactable object from the world.
    /// </summary>
    /// <param name="warnPlayer">If the player should be 
    /// warned about the interactable object.</param>
    private void GetInteractable (bool warnPlayer = true)
    {
        RaycastHit[] hits = Physics.RaycastAll(
            new Ray(origin.position, origin.forward), 
            interactDistance, 
            ~ignoreLayers);

        List<RaycastHit> hitList = new List<RaycastHit>(hits);
        hitList.Sort((x, y) => 
            Vector3.Distance(x.point, origin.position).
            CompareTo(Vector3.Distance(y.point, origin.position)));

        foreach (RaycastHit hit in hitList)
        {
            Interactable interactable = 
                hit.collider.gameObject.GetComponent<Interactable>();
            if (interactable != null)
            {
                if (warnPlayer)
                    GameInstance.HUD.EnableInteractMessage(true, interactable);

                Interactable = interactable;
                break;
            }
        }
    }
}
