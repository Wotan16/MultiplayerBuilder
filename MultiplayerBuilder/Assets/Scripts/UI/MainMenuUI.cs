using UnityEngine;
using UnityEngine.SceneManagement;
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
            MainSceneUI.Instance.SetMainSceneUIState(MainSceneUI.MainSceneUIState.Lobby);
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
