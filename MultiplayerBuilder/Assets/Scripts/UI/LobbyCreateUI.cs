using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour
{
    [SerializeField]
    private Toggle privateToggle;
    [SerializeField]
    private TMP_InputField lobbyNameField;
    [SerializeField]
    private Button createLobbyButton;
    [SerializeField]
    private Button closeButton;

    private void Awake()
    {
        createLobbyButton.onClick.AddListener(() =>
        {
            GameLobby.Instance.CreateLobby(lobbyNameField.text, privateToggle.isOn);
        });
        closeButton.onClick.AddListener(() =>
        {
            Hide();
        });

        Hide();
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
