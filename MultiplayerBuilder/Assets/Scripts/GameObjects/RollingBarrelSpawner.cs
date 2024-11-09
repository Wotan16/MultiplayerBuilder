using Unity.Netcode;
using UnityEngine;

public class RollingBarrelSpawner : MonoBehaviour
{
    [SerializeField]
    private RollingBarrel barrelPrefab;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SpawnBarrel();
        }
    }


    public void SpawnBarrel()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            RollingBarrel barrel = Instantiate(barrelPrefab, transform.position, transform.rotation);
            barrel.NetworkObject.Spawn();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.2f);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward);
    }
}
