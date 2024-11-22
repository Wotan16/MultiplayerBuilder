using UnityEngine;
using Unity.Cinemachine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField]
    private CinemachineCamera cameraCM;

    private void Awake()
    {
        Player.OnLocalInstanceSpawned += Player_OnLocalInstanceSpawned;
    }

    private void Player_OnLocalInstanceSpawned(object sender, System.EventArgs e)
    {
        cameraCM.Target.TrackingTarget = Player.LocalInstance.transform;
    }

    private void OnDestroy()
    {
        Player.OnLocalInstanceSpawned -= Player_OnLocalInstanceSpawned;
    }
}
