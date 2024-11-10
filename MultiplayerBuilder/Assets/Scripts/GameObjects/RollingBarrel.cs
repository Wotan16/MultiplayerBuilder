using Unity.Netcode;
using UnityEngine;

public class RollingBarrel : NetworkBehaviour
{
    private Rigidbody rb;
    private DestroyAfterTimeNetwork destroyAfterTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        destroyAfterTime = GetComponent<DestroyAfterTimeNetwork>();
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

    public void DestroyAfterTime(float time)
    {
        if(!IsServer)
            return;

        destroyAfterTime.SetDestroyTimer(time);
    }
}
