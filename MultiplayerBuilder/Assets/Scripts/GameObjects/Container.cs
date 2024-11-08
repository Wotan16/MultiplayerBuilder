using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(FollowTransform))]
[RequireComponent(typeof(MyTransformSync))]
public class Container : NetworkBehaviour, IInteractable, ICarriable
{
    public event EventHandler OnContainedResourceSOChanged;

    [SerializeField]
    private ContainerSO containerSO;
    public ContainerSO ContainerSO { get { return containerSO; } }

    private ResourceSO containedResourceSO;
    public ResourceSO ContainedResorceSO { get { return containedResourceSO; } }

    public bool IsCarried { get { return carryingPlayer != null; } }
    public bool IsEmpty { get { return containedResourceSO == null; } }
    public bool IsDisposable { get {return containerSO.isDisposable; } }

    [SerializeField]
    private Outline outline;
    [SerializeField]
    private WorldIconUI containerResourceUI;

    [SerializeField]
    private Transform leftHandPoint;
    [SerializeField]
    private Transform rightHandPoint;

    private Player carryingPlayer;
    private Rigidbody rb;
    private FollowTransform followTransform;
    private MyTransformSync worldTransformSync;
    
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        followTransform = GetComponent<FollowTransform>();
        worldTransformSync = GetComponent<MyTransformSync>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if(!IsServer)
            return;

        if (containerSO.filledByDefault)
        {
            AddResource(containerSO.containableResources.FirstOrDefault());
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        if (IsCarried)
        {
            carryingPlayer.DropItem();
        }
    }

    private void Start()
    {
        if (IsServer && !IsCarried)
            worldTransformSync.enableSync.Value = true;
        else
            rb.isKinematic = true;

        if(!IsEmpty)
            containerResourceUI.SetSpriteUI(ContainedResorceSO);

        containerResourceUI.Hide();
    }

    public void OnSelected()
    {
        if (!IsEmpty)
            containerResourceUI.Show();

        outline.enabled = true;
    }

    public void OnDeselected()
    {
        if (!IsCarried)
            containerResourceUI.Hide();
        outline.enabled = false;
    }

    public bool CanPlayerInteract(Player player)
    {
        return !IsCarried && !player.HandsBusy;
    }

    public void OnInteract(Player player)
    {
        PickUpByPlayer(player);
    }

    public void PickUpByPlayer(Player player)
    {
        if (!IsHost)
        {
            player.PickUpItem(this);
            rb.isKinematic = true;
            followTransform.TargetTransform = player.CarriedObjectParent;
            carryingPlayer = player;
        }

        SetCarriedPlayerServerRpc(player);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetCarriedPlayerServerRpc(NetworkBehaviourReference playerReference, ServerRpcParams serverRpcParams = default)
    {
        if (!playerReference.TryGet(out Player player))
            return;

        player.PickUpItem(this);
        rb.isKinematic = true;
        followTransform.TargetTransform = player.CarriedObjectParent;
        carryingPlayer = player;
        worldTransformSync.enableSync.Value = false;

        SetCarriedPlayerClientRpc(playerReference, serverRpcParams.Receive.SenderClientId);
    }

    [ClientRpc]
    private void SetCarriedPlayerClientRpc(NetworkBehaviourReference playerReference, ulong senderClientId)
    {
        if (IsHost || NetworkManager.Singleton.LocalClientId == senderClientId)
            return;

        if (!playerReference.TryGet(out Player player))
            return;

        player.PickUpItem(this);
        rb.isKinematic = true;
        followTransform.TargetTransform = player.CarriedObjectParent;
        carryingPlayer = player;
    }

    public void OnDrop()
    {
        if (!IsHost)
        {
            carryingPlayer = null;
            followTransform.TargetTransform = null;
            containerResourceUI.Hide();
        }
        DropItemServerRpc(rb.linearVelocity);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DropItemServerRpc(Vector3 linearVelocity, ServerRpcParams serverRpcParams = default)
    {
        carryingPlayer = null;
        followTransform.TargetTransform = null;
        worldTransformSync.enableSync.Value = true;
        rb.isKinematic = false;
        rb.linearVelocity = linearVelocity;
        containerResourceUI.Hide();
        DropItemClientRpc(serverRpcParams.Receive.SenderClientId);
    }

    [ClientRpc]
    private void DropItemClientRpc(ulong senderClientId)
    {
        if (IsHost || NetworkManager.Singleton.LocalClientId == senderClientId)
            return;

        carryingPlayer = null;
        followTransform.TargetTransform = null;
        containerResourceUI.Hide();
    }

    public bool CanContainResource(ResourceSO resourceSO)
    {
        foreach(ResourceSO containableResource in ContainerSO.containableResources)
        {
            if(containableResource == resourceSO)
                return true;
        }
        return false;
    }

    public void AddResource(ResourceSO resourceSO)
    {
        int resourceSOIndex = InteractableManager.GetResourceSOIndex(resourceSO);
        AddResourceServerRpc(resourceSOIndex);
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddResourceServerRpc(int resourceSOIndex)
    {
        ResourceSO resourceSO = InteractableManager.GetResourceSOFromIndex(resourceSOIndex);
        containedResourceSO = resourceSO;
        AddResourceClientRpc(resourceSOIndex);
        OnContainedResourceSOChanged?.Invoke(this, EventArgs.Empty);
    }

    [ClientRpc]
    private void AddResourceClientRpc(int resourceSOIndex)
    {
        ResourceSO resourceSO = InteractableManager.GetResourceSOFromIndex(resourceSOIndex);
        containedResourceSO = resourceSO;
        containerResourceUI.SetSpriteUI(ContainedResorceSO);
        containerResourceUI.Show();
        OnContainedResourceSOChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool CanAddResource(ResourceSO resourceSO)
    {
        return CanContainResource(resourceSO) && IsEmpty;
    }

    public bool TryAddResource(ResourceSO resourceSO)
    {
        if (CanAddResource(resourceSO))
        {
            AddResource(resourceSO);
            return true;
        }
        return false;
    }

    public void EmptyContainer()
    {
        EmptyContainerServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void EmptyContainerServerRpc()
    {
        if (IsDisposable)
        {
            NetworkObject.Despawn(true);
            return;
        }
        containedResourceSO = null;
        EmptyContainerClientRpc();
        OnContainedResourceSOChanged?.Invoke(this, EventArgs.Empty);
    }

    [ClientRpc]
    private void EmptyContainerClientRpc()
    {
        containedResourceSO = null;
        OnContainedResourceSOChanged?.Invoke(this, EventArgs.Empty);
        containerResourceUI.Hide();
    }

    public bool TryEmptyContainer()
    {
        if (!IsEmpty)
        {
            EmptyContainer();
            return true;
        }
        return false;
    }

    public static void SpawnContainer(ContainerSO containerSO, Vector3 position)
    {
        InteractableManager.SpawnContainer(containerSO, position);
    }

    public Transform GetLeftHandPoint()
    {
        return leftHandPoint;
    }

    public Transform GetRightHandPoint()
    {
        return rightHandPoint;
    }
    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
