using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField]
    private Button exitButton;
    [SerializeField]
    private Button playButton;

    private void Awake()
    {
        exitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
        playButton.onClick.AddListener(() =>
        {
            SceneLoader.Load(SceneLoader.Scene.LobbyScene);
        });
    }
}
