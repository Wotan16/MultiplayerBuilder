using Unity.Netcode;
using UnityEngine;

public class RollingBarrelSpawner : MonoBehaviour
{
    [SerializeField]
    private RollingBarrel barrelPrefab;
    [SerializeField]
    private float timeToDestroyBarrels;
    private Timer spawnTimer;
    [SerializeField]
    private float timeToSpawn;

    private bool IsServer { get { return NetworkManager.Singleton.IsServer; } }

    private void Start()
    {
        if (IsServer)
        {
            spawnTimer = new Timer(timeToSpawn);
            spawnTimer.OnTimerEnds += () =>
            {
                SpawnBarrel();
                spawnTimer.Reset(timeToSpawn);
            };
        }
    }

    public void SpawnBarrel()
    {
        if (IsServer)
        {
            RollingBarrel barrel = Instantiate(barrelPrefab, transform.position, transform.rotation);
            barrel.NetworkObject.Spawn();
            barrel.DestroyAfterTime(timeToDestroyBarrels);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.2f);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward);
    }

    private void OnDestroy()
    {
        if( spawnTimer != null ) 
            spawnTimer.Stop();
    }
}
