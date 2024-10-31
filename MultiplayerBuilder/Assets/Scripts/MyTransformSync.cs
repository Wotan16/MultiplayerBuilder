using Unity.Netcode;
using UnityEngine;

public class MyTransformSync : NetworkBehaviour
{
    private Rigidbody rb;
    private bool hasRB { get { return rb != null; } }
    public NetworkVariable<bool> enableSync = new NetworkVariable<bool>();

    private void Awake()
    {
        if (!TryGetComponent(out Rigidbody rb))
            return;
        this.rb = rb;
    }

    [ClientRpc()]
    private void UpdateTransformPositionClientRpc(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }

    [ClientRpc()]
    private void UpdateRigidbodyPositionClientRpc(Vector3 position, Quaternion rotation)
    {
        rb.MovePosition(position);
        rb.MoveRotation(rotation);
    }

    private void FixedUpdate()
    {
        if (!IsServer)
            return;

        if (!enableSync.Value)
            return;

        if (!hasRB)
            return;

        UpdateRigidbodyPositionClientRpc(transform.position, transform.rotation);
    }

    private void LateUpdate()
    {
        if (!IsServer)
            return;

        if (!enableSync.Value)
            return;

        if (hasRB)
            return;

        UpdateTransformPositionClientRpc(transform.position, transform.rotation);
    }
}
