using UnityEngine;
using UnityEngine.UI;

public class WorldIconUI : MonoBehaviour
{
    [SerializeField]
    private Image iconImage;
    [SerializeField]
    private GameObject checkmarkObject;
    [SerializeField]
    private GameObject questionmarkObject;

    private void Awake()
    {
        SetIconStatus(IconStatus.Default);
    }

    public void SetIconStatus(IconStatus iconStatus)
    {
        switch (iconStatus)
        {
            case IconStatus.Default:
                checkmarkObject.SetActive(false);
                questionmarkObject.SetActive(false);
                break;
            case IconStatus.Required:
                checkmarkObject.SetActive(false);
                questionmarkObject.SetActive(true);
                break;
            case IconStatus.Copleted:
                checkmarkObject.SetActive(true);
                questionmarkObject.SetActive(false);
                break;
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetSpriteUI(ResourceSO resourceSO)
    {
        if (resourceSO == null)
        {
            return;
        }

        iconImage.sprite = resourceSO.sprite;
    }

    public enum IconStatus
    {
        Default,
        Required,
        Copleted
    }
}
