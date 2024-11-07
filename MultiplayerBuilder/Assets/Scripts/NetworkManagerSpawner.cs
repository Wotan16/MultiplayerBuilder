using Unity.Netcode;
using UnityEngine;

public class NetworkManagerSpawner : MonoBehaviour
{
    [SerializeField]
    private NetworkManager networkManagerPrefab;

    private void Awake()
    {
        NetworkManager networkManager = Instantiate(networkManagerPrefab);
        networkManager.SetSingleton();
    }
}
