using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(FollowTransform))]
[RequireComponent(typeof(MyTransformSync))]
public class Container : NetworkBehaviour, IInteractable
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
    private ResourceIconUI containerResourceUI;

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

    private void Start()
    {
        if (IsServer)
            worldTransformSync.enableSync.Value = true;
        else
            rb.isKinematic = true;

        containerResourceUI.SetResourceUI(ContainedResorceSO);
        containerResourceUI.Hide();
    }

    public bool CanPlayerInteract(Player player)
    {
        return !IsCarried && !player.HandsBusy;
    }

    public void OnInteract(Player player)
    {
        SetCarriedPlayerServerRpc(player);
    }

    public void OnSelected()
    {
        if(!IsEmpty)
            containerResourceUI.Show();

        outline.enabled = true;
    }

    public void OnDeselected()
    {
        if(!IsCarried)
            containerResourceUI.Hide();
        outline.enabled = false;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetCarriedPlayerServerRpc(NetworkBehaviourReference playerReference)
    {
        if (!playerReference.TryGet(out Player player))
            return;

        player.PickUpItem(this);
        rb.isKinematic = true;
        followTransform.TargetTransform = player.CarriedObjectParent;
        carryingPlayer = player;
        worldTransformSync.enableSync.Value = false;

        SetCarriedPlayerClientRpc(playerReference);
    }

    [ClientRpc]
    private void SetCarriedPlayerClientRpc(NetworkBehaviourReference playerReference)
    {
        if (!playerReference.TryGet(out Player player))
            return;

        player.PickUpItem(this);
        rb.isKinematic = true;
        followTransform.TargetTransform = player.CarriedObjectParent;
        carryingPlayer = player;
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
        containerResourceUI.SetResourceUI(ContainedResorceSO);
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
        InteractableManager.Instance.SpawnContainer(containerSO, position);
    }
}
