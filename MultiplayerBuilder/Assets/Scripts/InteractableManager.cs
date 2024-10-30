using System;
using Unity.Netcode;
using UnityEngine;

public class InteractableManager : NetworkBehaviour
{
    public static InteractableManager Instance { get; private set; }

    [SerializeField]
    private PickupListSO pickupListSO;

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnPickup(PickupSO pickupSO, Vector3 position)
    {
        SpawnPickupServerRpc(GetPickupSOIndex(pickupSO), position);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPickupServerRpc(int pickupSOIndex, Vector3 position)
    {
        PickupSO pickupSO = GetPickupSOFromIndex(pickupSOIndex);
        Pickup pickup = Instantiate(pickupSO.prefab, position, Quaternion.identity);
        pickup.NetworkObject.Spawn();
    }

    private int GetPickupSOIndex(PickupSO pickupSO)
    {
        return pickupListSO.list.IndexOf(pickupSO);
    }
    private PickupSO GetPickupSOFromIndex(int index)
    {
        return pickupListSO.list[index];
    }
}
