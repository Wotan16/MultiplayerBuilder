using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class InteractableManager : NetworkBehaviour
{
    public static InteractableManager Instance { get; private set; }

    [SerializeField]
    private ContainerListSO containerListSO;
    [SerializeField]
    private ResourceListSO resourceListSO;
    [SerializeField]
    private WorldIconUI resourceIconPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public static void SpawnContainer(ContainerSO containerSO, Vector3 position)
    {
        Instance.SpawnContainerServerRpc(GetContainerSOIndex(containerSO), position);
    }

    public Container SpawnContainer_ServerOnly(ContainerSO containerSO, Vector3 position)
    {
        if (IsServer)
        {
            Container container = Instantiate(containerSO.prefab, position, Quaternion.identity);
            container.NetworkObject.Spawn();
            return container;
        }
        else 
            return null;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnContainerServerRpc(int containerSOIndex, Vector3 position)
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

    public static WorldIconUI CreateResourceIcon(Transform parent)
    {
        return Instantiate(Instance.resourceIconPrefab, parent);
    }
}
