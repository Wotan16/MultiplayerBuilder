using UnityEngine;

public class TestInteractable : MonoBehaviour, IInteractable
{
    [SerializeField]
    private GameObject defaultVisual;
    [SerializeField]
    private GameObject selectedVisual;

    private void Awake()
    {
        defaultVisual.SetActive(true);
        selectedVisual.SetActive(false);
    }

    public void OnInteract(Player player)
    {
        Debug.Log("player " + player.NetworkObject.GetInstanceID() + " interacted with " + gameObject.name);
    }

    public void OnDeselected()
    {
        defaultVisual.SetActive(true);
        selectedVisual.SetActive(false);
    }

    public void OnSelected()
    {
        defaultVisual.SetActive(false);
        selectedVisual.SetActive(true);
    }

    public bool CanPlayerInteract(Player player)
    {
        return true;
    }
}
