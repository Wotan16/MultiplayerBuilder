using UnityEngine;

public interface IInteractable
{
    void OnInteract(Player player);
    void OnSelected();
    void OnDeselected();
    bool CanPlayerInteract(Player player);
    GameObject GetGameObject();
}
