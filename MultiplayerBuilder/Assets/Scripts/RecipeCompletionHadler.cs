using System;
using System.Collections.Generic;
using UnityEngine;

public class RecipeCompletionHadler
{
    private RecipeSO recipeSO;
    private List<Ingredient> addedIgredients;

    public RecipeCompletionHadler(RecipeSO recipeSO)
    {
        this.recipeSO = recipeSO;
        addedIgredients = new List<Ingredient>();
        foreach(PickupSO pickupSO in recipeSO.recipeList)
        {
            addedIgredients.Add(new Ingredient(pickupSO));
        }
    }

    public void AddIngredient(PickupSO pickupSO)
    {
        foreach (Ingredient ingredient in addedIgredients)
        {
            if (ingredient.Contained)
                continue;
            if (ingredient.PickupSO != pickupSO)
                continue;

            ingredient.Contained = true;
            return;
        }
    }

    public bool IsRecipeCompleted()
    {
        foreach (Ingredient ingredient in addedIgredients)
        {
            if (!ingredient.Contained)
                return false;
        }
        return true;
    }

    public bool IsIngredientNeeded(PickupSO pickupSO)
    {
        foreach (Ingredient ingredient in addedIgredients)
        {
            if (ingredient.Contained)
                continue;
            if (ingredient.PickupSO != pickupSO)
                continue;
            return true;
        }
        return false;
    }
}

