using UnityEngine;

public class MainSceneUI : MonoBehaviour
{
    public static MainSceneUI Instance { get; private set; }

    public enum MainSceneUIState
    {
        MainMenu,
        Lobby,
        CreateLobby
    }

    [SerializeField]
    private MainMenuUI mainMenuUI;
    [SerializeField]
    private LobbyUI lobbyUI;
    [SerializeField]
    private LobbyCreateUI createUI;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetMainSceneUIState(MainSceneUIState.MainMenu);
    }

    public void SetMainSceneUIState(MainSceneUIState state)
    {
        switch (state)
        {
            case MainSceneUIState.MainMenu:
                mainMenuUI.Show();
                lobbyUI.Hide();
                createUI.Hide();
                break;

            case MainSceneUIState.Lobby:
                mainMenuUI.Hide();
                lobbyUI.Show();
                createUI.Hide();
                break;

            case MainSceneUIState.CreateLobby:
                mainMenuUI.Hide();
                lobbyUI.Hide();
                createUI.Show();
                break;
        }
    }
}
