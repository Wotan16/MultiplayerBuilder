using UnityEngine;

public class ConnectingUI : MonoBehaviour
{
    private void Start()
    {
        GameController.Instance.OnTryingToJoinGame += GameController_OnTryingToJoinGame;
        GameController.Instance.OnFailedToJoinGame += GameController_OnFailedToJoinGame;
        Hide();
    }

    private void GameController_OnFailedToJoinGame(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void GameController_OnTryingToJoinGame(object sender, System.EventArgs e)
    {
        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        GameController.Instance.OnTryingToJoinGame -= GameController_OnTryingToJoinGame;
        GameController.Instance.OnFailedToJoinGame -= GameController_OnFailedToJoinGame;
    }
}
