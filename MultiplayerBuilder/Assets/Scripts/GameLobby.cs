using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Multiplayer.Widgets;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class GameLobby : MonoBehaviour
{
    public static GameLobby Instance { get; private set; }

    private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";
    private const float HEARTBEAT_INTERVAL = 15f;

    private Lobby joinedLobby;
    private Timer heartbeatTimer;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeUnityAuthentication();

        heartbeatTimer = new Timer(HEARTBEAT_INTERVAL);
        heartbeatTimer.OnTimerEnds += HeartbeatTimer_OnTimerEnds;
    }

    private void HeartbeatTimer_OnTimerEnds()
    {
        if (IsLobbyHost())
        {
            heartbeatTimer.Reset(HEARTBEAT_INTERVAL);
            LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
        }
    }

    private async void InitializeUnityAuthentication()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions options = new InitializationOptions();
            options.SetProfile(Random.Range(0, 10000).ToString());
            await UnityServices.InitializeAsync(options);
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    private void Update()
    {
    }

    private bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private async Task<Allocation> AllocateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(GameController.MAX_PLAYER_NUMBER - 1);
            return allocation;
        }
        catch (RelayServiceException ex)
        {
            Debug.LogError(ex);
        }
        return default;
    }

    private async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try
        {
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return relayJoinCode;
        }
        catch (RelayServiceException ex)
        {
            Debug.LogError(ex);
            return default;
        }
    }

    private async Task<JoinAllocation> JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            return joinAllocation;
        }
        catch (RelayServiceException ex)
        {
            Debug.LogError(ex);
            return default;
        }
        
    }

    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        try
        {
            CreateLobbyOptions options = new CreateLobbyOptions { IsPrivate = isPrivate };
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, GameController.MAX_PLAYER_NUMBER, options);
            Allocation allocation = await AllocateRelay();

            string relayJoinCode = await GetRelayJoinCode(allocation);

            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                }
            });
        
            RelayServerData relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");
            UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            unityTransport.SetRelayServerData(relayServerData);

            GameController.Instance.StartHost();    
            SceneLoader.LoadNetwork(SceneLoader.Scene.CharacterSelectScene);
        }
        catch(LobbyServiceException ex)
        {
            Debug.LogError(ex);
        }
    }
        
    public async void QuickJoin()
    {
        try
        {
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

            RelayServerData relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            GameController.Instance.StartClient();  
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogError(ex);
        }
    }

    public async void JoinWithCode(string code)
    {
        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code);

            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

            RelayServerData relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            GameController.Instance.StartClient();
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogError(ex);
        }
    }

    public Lobby GetLobby()
    {
        return joinedLobby;
    }

    private void OnDestroy()
    {
        heartbeatTimer.OnTimerEnds -= HeartbeatTimer_OnTimerEnds;
    }

    public async void DeleteLobby()
    {
        if (joinedLobby == null)
            return;
        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
            joinedLobby = null;
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogError(ex);
        }
    }

    public async void LeaveLobby()
    {
        if (joinedLobby == null)
            return;
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogError(ex);
        }
    }
}
