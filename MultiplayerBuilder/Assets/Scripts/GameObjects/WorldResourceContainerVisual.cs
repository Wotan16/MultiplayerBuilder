using System.Collections.Generic;
using UnityEngine;

public class WorldResourceContainerVisual : MonoBehaviour
{
    [SerializeField]
    private List<Outline> outlines;
    [SerializeField]
    private Transform resourceIconsParent;

    public void UpdateResourceIcons(ResourceSO resourceSO, int numberOfUses)
    {
        ClearIcons();

        if (numberOfUses == 0 || resourceSO == null)
        {
            HideResourceIcons();
            return;
        }

        for (int i = 0; i < numberOfUses; i++)
        {
            WorldIconUI icon = InteractableManager.CreateResourceIcon(resourceIconsParent);
            icon.SetSpriteUI(resourceSO);
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
