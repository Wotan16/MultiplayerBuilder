using System;
using System.Collections.Generic;
using UnityEngine;

public class RecipeCompletionHadler
{
    private RecipeSO recipeSO;
    private List<Ingredient> addedIgredients;
    public List<Ingredient> AddedIgredients { get { return addedIgredients; } }

    public RecipeCompletionHadler(RecipeSO recipeSO)
    {
        this.recipeSO = recipeSO;
        addedIgredients = new List<Ingredient>();
        foreach(ResourceSO resourceSO in recipeSO.recipeList)
        {
            addedIgredients.Add(new Ingredient(resourceSO));
        }
    }

    public void AddIngredient(ResourceSO resourceSO)
    {
        foreach (Ingredient ingredient in addedIgredients)
        {
            if (ingredient.Contained)
                continue;
            if (ingredient.ResourceSO != resourceSO)
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

    public bool IsIngredientNeeded(ResourceSO resourceSO)
    {
        foreach (Ingredient ingredient in addedIgredients)
        {
            if (ingredient.Contained)
                continue;
            if (ingredient.ResourceSO != resourceSO)
                continue;
            return true;
        }
        return false;
    }
}

