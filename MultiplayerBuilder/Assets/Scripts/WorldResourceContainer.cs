using System;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class WorldResourceContainer : NetworkBehaviour, IInteractable
{
    public event EventHandler OnContainerEmpty;

    [SerializeField]
    private List<Outline> outlines;
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
        AddResourceToPlayerContainerServerRpc(player.CarriedContainer);
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddResourceToPlayerContainerServerRpc(NetworkBehaviourReference containerReference)
    {
        if (!containerReference.TryGet(out Container container))
            return;
        container.AddResource(containedResourceSO);
        if (!isInfinte)
        {
            numberOfUses.Value--;
            if(numberOfUses.Value == 0)
            {
                OnContainerEmpty?.Invoke(this, EventArgs.Empty);
            }
        }

        AddResourceToContainerClientRpc(containerReference);
    }

    [ClientRpc]
    private void AddResourceToContainerClientRpc(NetworkBehaviourReference containerReference)
    {
        if (!containerReference.TryGet(out Container container))
            return;
        container.AddResource(containedResourceSO);
    }

    public void SetNumberOfUses(int numberOfUses)
    {
        SetNumberOfUsesServerRpc(numberOfUses);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetNumberOfUsesServerRpc(int numberOfUses)
    {
        this.numberOfUses.Value = numberOfUses;
    }

    public void OnDeselected()
    {
        foreach (Outline outline in outlines)
        {
            outline.enabled = false;
        }
    }

    public void OnSelected()
    {
        foreach (Outline outline in outlines)
        {
            outline.enabled = true;
        }
    }

    public void SetContainedResource(ResourceSO resourceSO)
    {
        containedResourceSO = resourceSO;
    }
}
