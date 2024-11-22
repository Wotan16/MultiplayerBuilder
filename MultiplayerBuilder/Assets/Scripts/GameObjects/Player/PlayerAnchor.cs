using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;

public class PlayerAnchor : NetworkBehaviour
{
    private Player player;
    private Vector3 previousPosition;
    private Transform parent;
    public Vector3 movementDelta { get; private set; }

    private void Update()
    {
        movementDelta = transform.position - previousPosition;
        previousPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (this.player != null)
            return;

        if (other.gameObject != Player.LocalInstance.gameObject)
            return;

        if (!other.TryGetComponent(out Player player))
            return;

        player.SetAnchor(this);
        this.player = player;
    }

    private void OnTriggerExit(Collider other)
    {
        if (player == null)
            return;

        if (other.gameObject != player.gameObject)
            return;

        player.RemoveAnchor();  
        player = null;
    }

}
