using Unity.Netcode;
using UnityEngine;

public abstract class BaseCraftingStation : NetworkBehaviour, IInteractable
{
    [SerializeField]
    protected RecipeSO recipeSO;

    protected NetworkVariable<CraftingStationState> currentState = new NetworkVariable<CraftingStationState>();

    protected float timeToMixLeft;
    protected RecipeCompletionHadler recipeHandler;

    protected virtual void Awake()
    {
        currentState.Value = CraftingStationState.WaitingForIngridients;
        recipeHandler = new RecipeCompletionHadler(recipeSO);
    }

    private void Update()
    {
        if (!IsServer)
            return;

        if (currentState.Value == CraftingStationState.Crafting)
        {
            if (timeToMixLeft <= 0f)
            {
                currentState.Value = CraftingStationState.WaitingUntilEmpty;
                OnCraftingEnded();
                return;
            }
            timeToMixLeft -= Time.deltaTime;
            // set progress bar on host
            // invoke clientRpc to set porgress bar on clients
        }
    }

    protected abstract void OnCraftingEnded();

    public virtual bool CanPlayerInteract(Player player)
    {
        if (!player.HandsBusy)
            return false;

        return CanAddItem(player.CarriedItem.PickupSO);
    }

    public virtual void OnInteract(Player player)
    {
        int pickupIndex = InteractableManager.GetPickupSOIndex(player.CarriedItem.PickupSO);
        AddPickupServerRpc(player);
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddPickupServerRpc(NetworkBehaviourReference playerReference)
    {
        if (!playerReference.TryGet(out Player player))
            return;

        if (!CanAddItem(player.CarriedItem.PickupSO))
            return;

        recipeHandler.AddIngredient(player.CarriedItem.PickupSO);
        player.CarriedItem.NetworkObject.Despawn(true);

        if (recipeHandler.IsRecipeCompleted())
            StartCrafting();
    }

    private void StartCrafting()
    {
        currentState.Value = CraftingStationState.Crafting;
        timeToMixLeft = recipeSO.timeToMake;
        Debug.Log("StartMixing");
    }

    private bool CanAddItem(PickupSO pickupSO)
    {
        return currentState.Value == CraftingStationState.WaitingForIngridients &&
            recipeHandler.IsIngredientNeeded(pickupSO);
    }

    private float GetNormalizedCraftingTime()
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
