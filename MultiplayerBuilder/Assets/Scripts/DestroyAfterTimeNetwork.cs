using Unity.Netcode;
using UnityEngine;

public class DestroyAfterTimeNetwork : NetworkBehaviour
{
    private float timeToDestroyDelta;
    private bool destroyAfterTime = true;

    private void Update()
    {
        if (!IsServer)
            return;

        if (!destroyAfterTime)
            return;

        if (timeToDestroyDelta <= 0f)
        {
            NetworkObject.Despawn(true);
            return;
        }
        timeToDestroyDelta -= Time.deltaTime;
    }

    public void SetDestroyTimer(float time)
    {
        destroyAfterTime = true;
        timeToDestroyDelta = time;
    }
}
