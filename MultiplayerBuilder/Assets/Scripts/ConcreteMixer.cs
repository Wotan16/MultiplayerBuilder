using NUnit.Framework;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ConcreteMixer : NetworkBehaviour, IInteractable
{
    [SerializeField]
    private ConcreteMixerVisual visual;
    [SerializeField]
    private RecipeSO recipeSO;

    private enum ConcreteMixerState
    {
        WaitingForIngridients,
        Mixing,
        WaitingUntilEmpty
    }

    private NetworkVariable<ConcreteMixerState> currentState = new NetworkVariable<ConcreteMixerState>();

    private float timeToMixLeft;
    private RecipeCompletionHadler recipeHandler;

    private void Awake()
    {
        visual.DisableOutline();
        currentState.Value = ConcreteMixerState.WaitingForIngridients;
        recipeHandler = new RecipeCompletionHadler(recipeSO);
    }

    private void Update()
    {
        if (!IsServer)
            return;

        if(currentState.Value == ConcreteMixerState.Mixing)
        {
            if (timeToMixLeft <= 0f)
            {
                currentState.Value = ConcreteMixerState.WaitingUntilEmpty;
                //fill with concrete
                Debug.Log("Mixed");
                EndedMixingClientRpc();
                return;
            }
            timeToMixLeft -= Time.deltaTime;
            // set progress bar on host
            // invoke clientRpc to set porgress bar on clients
        }
    }

    
    public void OnSelected()
    {
        visual.EnableOutline();
    }

    public void OnDeselected()
    {
        visual.DisableOutline();
    }

    public bool CanPlayerInteract(Player player)
    {
        if(!player.HandsBusy)
            return false;

        return CanAddItem(player.CarriedItem.PickupSO);
    }

    public void OnInteract(Player player)
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
            StartMixing();
    }

    [ClientRpc]
    private void EndedMixingClientRpc()
    {
        if (IsHost)
            return;

        Debug.Log("Mixed");
    }

    private void StartMixing()
    {
        currentState.Value = ConcreteMixerState.Mixing;
        timeToMixLeft = recipeSO.timeToMake;
        Debug.Log("StartMixing");
    }

    private float GetNormalizedMixingTime()
    {
        return timeToMixLeft / recipeSO.timeToMake;
    }

    private bool CanAddItem(PickupSO pickupSO)
    {
        return currentState.Value == ConcreteMixerState.WaitingForIngridients &&
            recipeHandler.IsIngredientNeeded(pickupSO);
    }
}
