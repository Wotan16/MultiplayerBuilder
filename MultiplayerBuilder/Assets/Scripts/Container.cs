using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Container : NetworkBehaviour, IInteractable
{
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

    private Player carryingPlayer;
    private Rigidbody rb;
    private FollowTransform followTransform;
    private MyTransformSync worldTransformSync;
    

    private void Awake()
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
        outline.enabled = true;
    }

    public void OnDeselected()
    {
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

    [ServerRpc]
    private void AddResourceServerRpc(int resourceSOIndex)
    {
        ResourceSO resourceSO = InteractableManager.GetResourceSOFromIndex(resourceSOIndex);
        containedResourceSO = resourceSO;
        AddResourceClientRpc(resourceSOIndex);
    }

    [ClientRpc]
    private void AddResourceClientRpc(int resourceSOIndex)
    {
        ResourceSO resourceSO = InteractableManager.GetResourceSOFromIndex(resourceSOIndex);
        containedResourceSO = resourceSO;
    }

    public bool TryAddResource(ResourceSO resourceSO)
    {
        if (CanContainResource(resourceSO))
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

    [ServerRpc]
    private void EmptyContainerServerRpc()
    {
        Debug.Log("container empty");
        if (IsDisposable)
        {
            NetworkObject.Despawn(true);
            return;
        }
        containedResourceSO = null;
        EmptyContainerClientRpc();
    }

    [ClientRpc]
    private void EmptyContainerClientRpc()
    {
        containedResourceSO = null;
        Debug.Log("container empty");
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
