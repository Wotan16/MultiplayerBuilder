using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public enum Scene
    {
        MainMenuScene,
        TestScene,
        LobbyScene,
        CharacterSelectScene
    }

    private static Scene targetScene;

    public static void Load(Scene targetScene, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
    {
        SceneLoader.targetScene = targetScene;
        SceneManager.LoadScene(targetScene.ToString(), loadSceneMode);
    }

    public static void LoadNetwork(Scene targetScene)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
    }

    public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }
}
