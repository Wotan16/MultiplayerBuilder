using System;
using Unity.Netcode;
using UnityEngine;

public abstract class BaseCraftingStation : NetworkBehaviour, IInteractable
{
    public event EventHandler OnResourceAddedOnClient;
    public event EventHandler OnStartedCraftingOnClient;

    [SerializeField]
    protected RecipeSO recipeSO;

    protected NetworkVariable<CraftingStationState> currentState = new NetworkVariable<CraftingStationState>();
    protected CraftingStationState CurrentState { get { return currentState.Value; } }
    protected float timeToMixLeft;
    protected RecipeCompletionHadler recipeHandler;

    [SerializeField]
    private Collider craftingStationCollider;
    [SerializeField]
    private Collider worldResourceContainerCollider;
    [SerializeField]
    protected WorldResourceContainer worldResourceContainer;
    [SerializeField]
    private ResourceSO outputResource;
    [SerializeField]
    private int amountOfResource;

    protected virtual void Awake()
    {
        currentState.Value = CraftingStationState.WaitingForIngridients;
        recipeHandler = new RecipeCompletionHadler(recipeSO);
        craftingStationCollider.enabled = true;
        worldResourceContainerCollider.enabled = false;
        worldResourceContainer.SetContainedResource(outputResource);
    }

    protected virtual void Start()
    {
        if (IsServer)
        {
            worldResourceContainer.OnContainerEmpty += WorldResourceContainer_OnContainerEmpty;
        }
    }

    private void Update()
    {
        if (!IsServer)
            return;

        if (currentState.Value == CraftingStationState.Crafting)
        {
            if (timeToMixLeft <= 0f)
            {
                OnCraftingEnded();
                return;
            }
            timeToMixLeft -= Time.deltaTime;
            // set progress bar on host
            // invoke clientRpc to set porgress bar on clients
        }
    }

    private void WorldResourceContainer_OnContainerEmpty(object sender, System.EventArgs e)
    {
        currentState.Value = CraftingStationState.WaitingForIngridients;
        recipeHandler = new RecipeCompletionHadler(recipeSO);
        OnContainerEmptyClientRpc();
    }

    [ClientRpc]
    protected virtual void OnContainerEmptyClientRpc()
    {
        if (!IsHost)
            recipeHandler = new RecipeCompletionHadler(recipeSO);

        craftingStationCollider.enabled = true;
        worldResourceContainerCollider.enabled = false;
        Debug.Log("Container Empty");
    }

    protected virtual void OnCraftingEnded()
    {
        currentState.Value = CraftingStationState.WaitingUntilEmpty;
        worldResourceContainer.SetNumberOfUses(amountOfResource);
        OnCraftingEndedClientRpc();
    }

    [ClientRpc]
    private void OnCraftingEndedClientRpc()
    {
        worldResourceContainerCollider.enabled = true;
        craftingStationCollider.enabled = false;
        Debug.Log("Mixed");
    }

    public virtual bool CanPlayerInteract(Player player)
    {
        if (!player.HandsBusy)
            return false;

        return CanAddResource(player.CarriedContainer.ContainedResorceSO);
    }

    public virtual void OnInteract(Player player)
    {
        AddResourceServerRpc(player);
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddResourceServerRpc(NetworkBehaviourReference playerReference)
    {
        if (!playerReference.TryGet(out Player player))
            return;

        if (!CanAddResource(player.CarriedContainer.ContainedResorceSO))
            return;

        recipeHandler.AddIngredient(player.CarriedContainer.ContainedResorceSO);

        int resourceIndex = InteractableManager.GetResourceSOIndex(player.CarriedContainer.ContainedResorceSO);
        AddResourceClientRpc(resourceIndex);
        player.CarriedContainer.EmptyContainer();

        if (recipeHandler.IsRecipeCompleted())
            StartCrafting();
    }

    [ClientRpc]
    private void AddResourceClientRpc(int resourceIndex)
    {
        ResourceSO resourceSO = InteractableManager.GetResourceSOFromIndex(resourceIndex);
        recipeHandler.AddIngredient(resourceSO);
        OnResourceAddedOnClient?.Invoke(this, EventArgs.Empty);
    }

    private void StartCrafting()
    {
        currentState.Value = CraftingStationState.Crafting;
        timeToMixLeft = recipeSO.timeToMake;
        Debug.Log("StartCrafting");
        StartCraftingClientRpc();
    }

    [ClientRpc]
    private void StartCraftingClientRpc()
    {
        OnStartedCraftingOnClient?.Invoke(this, EventArgs.Empty);
    }

    private bool CanAddResource(ResourceSO resourceSO)
    {
        return currentState.Value == CraftingStationState.WaitingForIngridients &&
            recipeHandler.IsIngredientNeeded(resourceSO);
    }

    protected float GetNormalizedCraftingTime()
    {
        return timeToMixLeft / recipeSO.timeToMake;
    }

    public abstract void OnDeselected();

    public abstract void OnSelected();

    protected enum CraftingStationState
    {
        WaitingForIngridients,
        Crafting,
        WaitingUntilEmpty
    }
}
