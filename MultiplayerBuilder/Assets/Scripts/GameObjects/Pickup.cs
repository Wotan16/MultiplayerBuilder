using System;
using Unity.Netcode;
using UnityEngine;

public class Pickup : NetworkBehaviour, IInteractable
{
    public event EventHandler OnPickupDespawned;

    [SerializeField]
    private PickupSO pickupSO;
    public PickupSO PickupSO { get { return pickupSO; } }
    [SerializeField]
    private Outline outline;

    private Player carryingPlayer;
    private Rigidbody rb;
    private FollowTransform followTransform;
    private MyTransformSync worldTransformSync;

    private bool isContainer;

    public bool IsCarried { get { return carryingPlayer != null; } }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        followTransform = GetComponent<FollowTransform>();
        worldTransformSync = GetComponent<MyTransformSync>();
    }

    private void Start()
    {
        if (IsServer)
            worldTransformSync.enableSync.Value = true;
        else
            rb.isKinematic = true;
    }

    public bool CanPlayerInteract(Player player)
    {
        return !IsCarried && !player.HandsBusy;
    }

    public void OnDeselected()
    {
        outline.enabled = false;
    }

    public void OnInteract(Player player)
    {
        SetCarriedPlayerServerRpc(player);
    }

    public void OnSelected()
    {
        outline.enabled = true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetCarriedPlayerServerRpc(NetworkBehaviourReference playerReference)
    {
        //if (!playerReference.TryGet(out Player player))
        //    return;

        //player.PickUpItem(this);
        //rb.isKinematic = true;
        //followTransform.TargetTransform = player.CarriedObjectParent;
        //carryingPlayer = player;
        //worldTransformSync.enableSync.Value = false;

        //SetCarriedPlayerClientRpc(playerReference);
    }

    [ClientRpc]
    private void SetCarriedPlayerClientRpc(NetworkBehaviourReference playerReference)
    {
        //if (!playerReference.TryGet(out Player player))
        //    return;

        //player.PickUpItem(this);
        //rb.isKinematic = true;
        //followTransform.TargetTransform = player.CarriedObjectParent;
        //carryingPlayer = player;
    }

    public void OnDrop()
    {
        DropItemServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DropItemServerRpc()
    {
        carryingPlayer = null;
        followTransform.TargetTransform = null;
        worldTransformSync.enableSync.Value = true;
        rb.isKinematic = false;
        DropItemClientRpc();
    }

    [ClientRpc]
    private void DropItemClientRpc()
    {
        carryingPlayer = null;
        followTransform.TargetTransform = null;
    }

    public static void SpawnPickup(PickupSO pickupSO, Vector3 position)
    {
        //InteractableManager.Instance.SpawnPickup(pickupSO, position);
    }

    public override void OnNetworkDespawn()
    {
        OnPickupDespawned?.Invoke(this, EventArgs.Empty);
        base.OnNetworkDespawn();
    }
}