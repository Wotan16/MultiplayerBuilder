using System.Collections.Generic;
using UnityEngine;

public class QuestObjectVisual : MonoBehaviour
{
    [SerializeField]
    private List<Outline> outlines;
    [SerializeField]
    private Transform resourceIconsParent;
    [SerializeField]
    private ResourceIconUI iconPrefab;

    public void UpdateResourceIcons(RecipeCompletionHadler recipeCompletionHadler)
    {
        ClearIcons();

        foreach (Ingredient ingredient in recipeCompletionHadler.AddedIgredients)
        {
            ResourceIconUI icon = Instantiate(iconPrefab, resourceIconsParent);
            icon.SetResourceUI(ingredient.ResourceSO);

            ResourceIconUI.IconStatus iconStatus = ingredient.Contained ?
                ResourceIconUI.IconStatus.Copleted : ResourceIconUI.IconStatus.Required;

            icon.SetIconStatus(iconStatus);
            icon.Show();
        }
    }

    private void ClearIcons()
    {
        foreach (Transform child in resourceIconsParent)
        {
            Destroy(child.gameObject);
        }
    }

    public void ShowResourceIcons()
    {
        resourceIconsParent.gameObject.SetActive(true);
    }

    public void HideResourceIcons()
    {
        resourceIconsParent.gameObject.SetActive(false);
    }

    public void EnableOutline()
    {
        foreach (Outline outline in outlines)
        {
            outline.enabled = true;
        }
    }

    public void DisableOutline()
    {
        foreach (Outline outline in outlines)
        {
            outline.enabled = false;
        }
    }
}
