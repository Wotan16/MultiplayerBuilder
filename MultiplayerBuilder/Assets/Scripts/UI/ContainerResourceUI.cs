using UnityEngine;
using UnityEngine.UI;

public class ContainerResourceUI : MonoBehaviour
{
    [SerializeField]
    private Image resourceImage;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetResourceUI(ResourceSO resourceSO)
    {
        if (resourceSO == null)
        {
            return;
        }

        resourceImage.sprite = resourceSO.sprite;
    }
}
