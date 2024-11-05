using UnityEngine;
using UnityEngine.UI;

public class IngredientIconUI : MonoBehaviour
{
    [SerializeField]
    private Image resourceImage;
    [SerializeField]
    private GameObject backgroundObjectDefault;
    [SerializeField]
    private GameObject backgroundObjectCompleted;
    [SerializeField]
    private GameObject toggleObject;

    private void Awake()
    {
        SetIsCompleted(false);
    }

    public void SetIsCompleted(bool isCompleted)
    {
        backgroundObjectDefault.SetActive(!isCompleted);
        backgroundObjectCompleted.SetActive(isCompleted);
        toggleObject.SetActive(isCompleted);
    }

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
