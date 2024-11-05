using System.Collections.Generic;
using UnityEngine;

public class WorldResourceContainerVisual : MonoBehaviour
{
    [SerializeField]
    private List<Outline> outlines;
    [SerializeField]
    private Transform resourceIconsParent;
    [SerializeField]
    private ContainerResourceUI iconPrefab;

    public void UpdateResourceIcons(ResourceSO resourceSO, int numberOfUses)
    {
        ClearIcons();

        if (numberOfUses == 0)
        {
            HideResourceIcons();
            return;
        }

        for (int i = 0; i < numberOfUses; i++)
        {
            ContainerResourceUI icon = Instantiate(iconPrefab, resourceIconsParent);
            icon.SetResourceUI(resourceSO);
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
