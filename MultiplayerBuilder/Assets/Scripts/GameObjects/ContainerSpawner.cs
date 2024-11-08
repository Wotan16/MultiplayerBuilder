using System;
using Unity.Netcode;
using UnityEngine;

public class ContainerSpawner : NetworkBehaviour, IInteractable
{
    public event EventHandler OnContainerEmpty;
    public event EventHandler OnResourceRemoved;

    [SerializeField]
    private ContainerSO containerSO;
    [SerializeField]
    private NetworkVariable<int> numberOfUses = new NetworkVariable<int>();
    [SerializeField]
    private bool isInfinte;
    [SerializeField]
    private ContainerSpawnerVisual visual;
    [SerializeField]
    private Collider coll;

    private void Start()
    {
        numberOfUses.OnValueChanged += NumberOfUses_OnValueChanged;
        UpdateIcons(numberOfUses.Value);
        visual.DisableOutline();
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
        visual.UpdateResourceIcons(containerSO, current);
    }

    public bool CanPlayerInteract(Player player)
    {
        return !player.HandsBusy && (numberOfUses.Value > 0 || isInfinte);
    }

    public void OnInteract(Player player)
    {
        SpawnContainerInPlayerHandsServerRpc(player);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnContainerInPlayerHandsServerRpc(NetworkBehaviourReference playerReference)
    {
        if (!playerReference.TryGet(out Player player))
            return;

        Container container = InteractableManager.Instance.SpawnContainer_ServerOnly(containerSO, transform.position);
        container.PickUpByPlayer(player);

        if (!isInfinte)
        {
            numberOfUses.Value--;
            if (numberOfUses.Value == 0)
            {
                OnContainerEmpty?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                OnResourceRemoved?.Invoke(this, EventArgs.Empty);
            }
        }
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

    public void SetContainer(ContainerSO containerSO)
    {
        this.containerSO = containerSO;
    }

    public void EnableCollider(bool value)
    {
        coll.enabled = value;
    }
    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
