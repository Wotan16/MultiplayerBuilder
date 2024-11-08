using System;
using Unity.Netcode;
using UnityEngine;

public class QuestObject : NetworkBehaviour,  IInteractable
{
    public event EventHandler OnRecipeCompleted_Server;

    [SerializeField]
    private QuestObjectVisual visual;
    [SerializeField]
    private Collider coll;

    private RecipeCompletionHadler recipeHandler;

    private void Awake()
    {
        visual.DisableOutline();
    }

    public void SetRecipe(RecipeSO recipeSO)
    {
        recipeHandler = new RecipeCompletionHadler(recipeSO);
        visual.ShowResourceIcons();
        visual.UpdateResourceIcons(recipeHandler);
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
        {
            OnRecipeCompletedClientRpc();
            OnRecipeCompleted_Server?.Invoke(this, EventArgs.Empty);
        }
    }

    [ClientRpc]
    public void AddResourceClientRpc(int resourceIndex)
    {
        if (!IsHost)
        {
            ResourceSO resourceSO = InteractableManager.GetResourceSOFromIndex(resourceIndex);
            recipeHandler.AddIngredient(resourceSO);
        }
        visual.ShowResourceIcons();
        visual.UpdateResourceIcons(recipeHandler);
    }

    [ClientRpc]
    public void OnRecipeCompletedClientRpc()
    {
        visual.HideResourceIcons();
    }

    public bool CanAddResource(ResourceSO resourceSO)
    {
        return recipeHandler.IsIngredientNeeded(resourceSO);
    }

    public bool CanPlayerInteract(Player player)
    {
        if (!player.HandsBusy)
            return false;

        return CanAddResource(player.CarriedContainer.ContainedResorceSO);
    }

    public void OnSelected()
    {
        visual.EnableOutline();
    }

    public void OnDeselected()
    {
        visual.DisableOutline();
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
