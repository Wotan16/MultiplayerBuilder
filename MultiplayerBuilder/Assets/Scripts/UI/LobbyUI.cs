using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField]
    private Button cerateGameButton;
    [SerializeField]
    private Button quickJoinButton;
    [SerializeField]
    private LobbyCreateUI lobbyCreateUI;
    [SerializeField]
    private TMP_InputField lobbyCodeField;
    [SerializeField]
    private Button joinWithCodeButton;
    [SerializeField]
    private TMP_InputField playerNameField;

    private void Awake()
    {
        cerateGameButton.onClick.AddListener(() =>
        {
            lobbyCreateUI.Show();
        });
        quickJoinButton.onClick.AddListener(() =>
        {
            GameLobby.Instance.QuickJoin();
        });
        joinWithCodeButton.onClick.AddListener(() =>
        {
            GameLobby.Instance.JoinWithCode(lobbyCodeField.text);
        });
    }

    private void Start()
    {
        playerNameField.text = GameController.Instance.GetPlayerName();
        playerNameField.onValueChanged.AddListener((string newText) =>
        {
            GameController.Instance.SetPlayerName(newText);
        });
    }
}
