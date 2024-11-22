using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class WinScreenUI : MonoBehaviour
{
    private void Start()
    {
        GlobalQuest quest = FindAnyObjectByType<GlobalQuest>();
        quest.OnGlobalQuestCompleted += Quest_OnGlobalQuestCompleted;

        Hide();
    }

    private void Quest_OnGlobalQuestCompleted(object sender, System.EventArgs e)
    {
        Show();
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
