using System;
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
    private TMP_InputField lobbyCodeField;
    [SerializeField]
    private Button joinWithCodeButton;
    [SerializeField]
    private TMP_InputField playerNameField;

    [SerializeField]
    private MainMenuUI mainMenuUI;
    [SerializeField]
    private Button backToMenuButton;

    private void Awake()
    {
        cerateGameButton.onClick.AddListener(() =>
        {
            MainSceneUI.Instance.SetMainSceneUIState(MainSceneUI.MainSceneUIState.CreateLobby);
        });
        quickJoinButton.onClick.AddListener(() =>
        {
            GameLobby.Instance.QuickJoin();
        });
        joinWithCodeButton.onClick.AddListener(() =>
        {
            GameLobby.Instance.JoinWithCode(lobbyCodeField.text);
        });
        backToMenuButton.onClick.AddListener(() =>
        {
            MainSceneUI.Instance.SetMainSceneUIState(MainSceneUI.MainSceneUIState.MainMenu);
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

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
