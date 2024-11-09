using Unity.Netcode;
using UnityEngine;

public class RollingBarrel : NetworkBehaviour
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        if (!other.TryGetComponent(out Player player))
            return;

        Vector3 impulseForce = rb.mass * rb.linearVelocity;
        player.KillPlayer(impulseForce);
    }
}
