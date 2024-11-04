using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class WorldResourceContainer : NetworkBehaviour, IInteractable
{
    [SerializeField]
    private Outline outline;
    [SerializeField]
    private ResourceSO containedResourceSO;
    [SerializeField]
    private NetworkVariable<int> numberOfUses = new NetworkVariable<int>();
    [SerializeField]
    private bool isInfinte;

    public bool CanPlayerInteract(Player player)
    {
        if (!player.HandsBusy)
            return false;

        return CanAddResourceToContainer(player.CarriedContainer);
    }

    private bool CanAddResourceToContainer(Container container)
    {
        return container.CanAddResource(containedResourceSO) && numberOfUses.Value > 0;
    }

    public void OnInteract(Player player)
    {
        AddResourceToContainerServerRpc(player.CarriedContainer);
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddResourceToContainerServerRpc(NetworkBehaviourReference containerReference)
    {
        if (!containerReference.TryGet(out Container container))
            return;
        container.AddResource(containedResourceSO);
        if (!isInfinte)
            numberOfUses.Value--;

        AddResourceToContainerClientRpc(containerReference);
    }

    [ClientRpc]
    private void AddResourceToContainerClientRpc(NetworkBehaviourReference containerReference)
    {
        if (!containerReference.TryGet(out Container container))
            return;
        container.AddResource(containedResourceSO);
    }

    public void OnDeselected()
    {
        outline.enabled = false;
    }

    public void OnSelected()
    {
        outline.enabled = true;
    }
}
