using Unity.Netcode;
using UnityEngine;

public class Pickup : NetworkBehaviour, IInteractable
{
    [SerializeField]
    private PickupSO pickupSO;
    [SerializeField]
    private Outline outline;

    private Player carryingPlayer;
    private Rigidbody rb;
    private FollowTransform followTransform;

    public bool IsCarried { get { return carryingPlayer != null; } }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        followTransform = GetComponent<FollowTransform>();
    }

    public bool CanPlayerInteract(Player player)
    {
        return !IsCarried;
    }

    public void OnDeselected()
    {
        outline.enabled = false;
    }

    public void OnInteract(Player player)
    {
        SetCarriedPlayerServerRpc(player);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetCarriedPlayerServerRpc(NetworkBehaviourReference playerReference)
    {
        if (!playerReference.TryGet(out Player player))
            return;

        player.PickUpItem(this);
        rb.isKinematic = true;
        followTransform.TargetTransform = player.InteractionPoint;
        carryingPlayer = player;
    }

    public void OnSelected()
    {
        outline.enabled = true;
    }

    public static void SpawnPickup(PickupSO pickupSO, Vector3 position)
    {
        InteractableManager.Instance.SpawnPickup(pickupSO, position);
    }
}