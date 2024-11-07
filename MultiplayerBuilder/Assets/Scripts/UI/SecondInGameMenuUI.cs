using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SecondInGameMenuUI : MonoBehaviour
{
    [SerializeField]
    private Button disconnectButton;

    private void Awake()
    {
        disconnectButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            SceneLoader.Load(SceneLoader.Scene.LobbyScene);
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
