using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WorldResourceContainer : NetworkBehaviour, IInteractable
{
    public event EventHandler OnContainerEmpty;
    public event EventHandler OnResourceRemoved;

    [SerializeField]
    private ResourceSO containedResourceSO;
    [SerializeField]
    private NetworkVariable<int> numberOfUses = new NetworkVariable<int>();
    [SerializeField]
    private bool isInfinte;
    [SerializeField]
    private WorldResourceContainerVisual visual;
    [SerializeField]
    private Collider coll;

    private void Start()
    {
        numberOfUses.OnValueChanged += NumberOfUses_OnValueChanged;
        UpdateIcons(numberOfUses.Value);
    }

    private void NumberOfUses_OnValueChanged(int previous, int current)
    {
        UpdateIcons(current);
    }

    private void UpdateIcons(int current)
    {
        if (current == 0)
        {
            visual.HideResourceIcons();
            return;
        }

        visual.ShowResourceIcons();
        visual.UpdateResourceIcons(containedResourceSO, current);
    }

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
            else
            {
                OnResourceRemoved?.Invoke(this, EventArgs.Empty);
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
        if (!IsServer)
            return;

        this.numberOfUses.Value = numberOfUses;
    }

    public void OnDeselected()
    {
        visual.DisableOutline();
    }

    public void OnSelected()
    {
        visual.EnableOutline();
    }

    public void SetContainedResource(ResourceSO resourceSO)
    {
        containedResourceSO = resourceSO;
    }

    public void EnableCollider(bool value)
    {
        coll.enabled = value;
    }
}
