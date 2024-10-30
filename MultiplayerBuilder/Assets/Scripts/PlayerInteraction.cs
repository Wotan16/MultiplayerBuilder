using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerInteraction : NetworkBehaviour
{
    [SerializeField]
    private float radius;
    [SerializeField]
    private Transform interactionPoint;
    public Transform InteractionPoint {  get { return interactionPoint; } }
    [SerializeField]
    private LayerMask interactableMask;
    private IInteractable selectedInteractable;
    public IInteractable SelectedInteractable { get { return selectedInteractable; } }

    private void Update()
    {
        if (!IsOwner || !IsSpawned) return;

        IInteractable nearestInteractable = GetNearestInteractable();

        if (nearestInteractable == null)
        {
            DeselectSelectedInteractable();
            return;
        }

        if (nearestInteractable == selectedInteractable)
        {
            if (!nearestInteractable.CanPlayerInteract(Player.LocalInstance))
            {
                DeselectSelectedInteractable();
            }
            return;
        }

        if (nearestInteractable.CanPlayerInteract(Player.LocalInstance))
        {
            DeselectSelectedInteractable();
            selectedInteractable = nearestInteractable;
            selectedInteractable.OnSelected();
        }
    }

    private void DeselectSelectedInteractable()
    {
        if(selectedInteractable != null)
        {
            selectedInteractable.OnDeselected();
            selectedInteractable = null;
        }
    }

    private IInteractable GetNearestInteractable()
    {
        Collider[] hits = Physics.OverlapSphere(interactionPoint.position, radius, interactableMask);
        IInteractable nearestInteractable = null;
        float currentShortestDistance = Mathf.Infinity;
        foreach(Collider hit in hits)
        {
            if (!hit.TryGetComponent(out IInteractable interactable))
                continue;

            float distanceToInteractable = Vector3.Distance(interactionPoint.position, hit.transform.position);
            if (distanceToInteractable < currentShortestDistance)
            {
                currentShortestDistance = distanceToInteractable;
                nearestInteractable = interactable;
            }
        }
        return nearestInteractable;
    }

    private void OnDrawGizmos() 
    {
        if (interactionPoint == null)
            return;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(interactionPoint.position, radius);
    }
}
