using UnityEngine;

public class KillCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        if (!other.TryGetComponent(out Player player))
            return;

        player.KillPlayer();
    }
}
