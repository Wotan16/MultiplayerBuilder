using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterSelectionPlayer : NetworkBehaviour
{
    [SerializeField]
    private int index;
    [SerializeField]
    private GameObject readyObject;
    [SerializeField]
    private TextMeshPro playerName;

    private void Start()
    {
        GameController.Instance.OnPlayerDataNetworkListChanged += GameController_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyChanged += CharacterSelectReady_OnReadyChanged;

        UpdatePlayer();
    }

    private void CharacterSelectReady_OnReadyChanged(object sender, System.EventArgs e)
    {
        UpdatePlayer();
    }

    private void GameController_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e)
    {
        UpdatePlayer();
    }

    public void UpdatePlayer()
    {
        if (GameController.Instance.IsPlayerIndexConnected(index))
        {
            Show();
            PlayerData data = GameController.Instance.GetPlayerDataFromIndex(index);
            readyObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(data.clientId));
            playerName.text = data.playerName.ToString();
        }
        else
            Hide();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    public override void OnNetworkDespawn()
    {
        GameController.Instance.OnPlayerDataNetworkListChanged -= GameController_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyChanged -= CharacterSelectReady_OnReadyChanged;
        base.OnNetworkDespawn();
    }
}
