using System;
using Unity.Netcode;
using UnityEngine;

public class InteractableManager : NetworkBehaviour
{
    public static InteractableManager Instance { get; private set; }

    [SerializeField]
    private ContainerListSO containerListSO;
    [SerializeField]
    private ResourceListSO resourceListSO;

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnContainer(ContainerSO containerSO, Vector3 position)
    {
        SpawnPickupServerRpc(GetContainerSOIndex(containerSO), position);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPickupServerRpc(int containerSOIndex, Vector3 position)
    {
        ContainerSO containerSO = GetContainerSOFromIndex(containerSOIndex);
        Container container = Instantiate(containerSO.prefab, position, Quaternion.identity);
        container.NetworkObject.Spawn();
    }

    public static int GetContainerSOIndex(ContainerSO containerSO)
    {
        return Instance.containerListSO.list.IndexOf(containerSO);
    }

    public static ContainerSO GetContainerSOFromIndex(int index)
    {
        return Instance.containerListSO.list[index];
    }

    public static int GetResourceSOIndex(ResourceSO resourceSO)
    {
        return Instance.resourceListSO.list.IndexOf(resourceSO);
    }

    public static ResourceSO GetResourceSOFromIndex(int index)
    {
        return Instance.resourceListSO.list[index];
    }
}
