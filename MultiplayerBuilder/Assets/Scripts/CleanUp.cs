using Unity.Netcode;
using UnityEngine;

public class CleanUp : MonoBehaviour
{
    private void Awake()
    {
        if(GameController.Instance != null)
        {
            Destroy(GameController.Instance.gameObject);
        }
        if(NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }
        if (GameLobby.Instance != null)
        {
            Destroy(GameLobby.Instance.gameObject);
        }
        if(UpdateProxy.Instance != null)
        {
            Destroy(UpdateProxy.Instance.gameObject);
        }
    }
}
