using System;
using Unity.Netcode;
using UnityEngine;

public class BaseCraftingStation : NetworkBehaviour
{
    public event EventHandler OnStartedCraftingOnClient;

    [SerializeField]
    protected RecipeSO recipeSO;

    protected NetworkVariable<CraftingStationState> currentState = new NetworkVariable<CraftingStationState>();
    protected CraftingStationState CurrentState { get { return currentState.Value; } }
    protected float timeToMixLeft;

    [SerializeField]
    protected WorldResourceContainer worldResourceContainer;
    [SerializeField]
    private QuestObject questObject;
    [SerializeField]
    private ResourceSO outputResource;
    [SerializeField]
    private int amountOfResource;

    protected virtual void Awake()
    {
        currentState.Value = CraftingStationState.WaitingForIngridients;
        worldResourceContainer.SetContainedResource(outputResource);
    }

    protected virtual void Start()
    {
        if (IsServer)
        {
            worldResourceContainer.OnContainerEmpty += WorldResourceContainer_OnContainerEmpty;
            questObject.OnRecipeCompleted += QuestObject_OnRecipeCompleted;
        }

        worldResourceContainer.EnableCollider(false);
        questObject.EnableCollider(true);
        questObject.SetRecipe(recipeSO);
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
        questObject.SetRecipe(recipeSO);
        OnContainerEmptyClientRpc();
    }

    [ClientRpc]
    protected virtual void OnContainerEmptyClientRpc()
    {
        if (!IsHost)
            questObject.SetRecipe(recipeSO);

        worldResourceContainer.EnableCollider(false);
        questObject.EnableCollider(true);
    }

    private void QuestObject_OnRecipeCompleted(object sender, EventArgs e)
    {
        StartCrafting();
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

    protected virtual void OnCraftingEnded()
    {
        currentState.Value = CraftingStationState.WaitingUntilEmpty;
        worldResourceContainer.SetNumberOfUses(amountOfResource);
        OnCraftingEndedClientRpc();
    }

    [ClientRpc]
    private void OnCraftingEndedClientRpc()
    {
        worldResourceContainer.EnableCollider(true);
        questObject.EnableCollider(false);
        Debug.Log("Mixed");
    }

    protected float GetNormalizedCraftingTime()
    {
        return timeToMixLeft / recipeSO.timeToMake;
    }

    protected enum CraftingStationState
    {
        WaitingForIngridients,
        Crafting,
        WaitingUntilEmpty
    }
}
