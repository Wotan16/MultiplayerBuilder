using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GlobalQuest : NetworkBehaviour
{
    public event EventHandler OnGlobalQuestCompleted;

    [SerializeField]
    private List<BuildingQuest> buildingQuests;
    private BuildingQuest activeQuest { get { return buildingQuests[0]; } }

    private void Start()
    {
        foreach (BuildingQuest quest in buildingQuests)
        {
            if (IsServer)
            {
                quest.OnQuestComplete_Server += Quest_OnQuestComplete_Server;
            }

            bool isActiveQuest = quest == activeQuest;
            quest.gameObject.SetActive(isActiveQuest);
        }
    }

    private void Quest_OnQuestComplete_Server(object sender, System.EventArgs e)
    {
        if (buildingQuests.Count <= 1)
        {
            OnAllQuestsCompletedRpc();
            return;
        }

        OnQuestCompleteClientRpc();
    }

    [Rpc(SendTo.NotServer)]
    private void OnQuestCompleteClientRpc()
    {
        SetNextQuestActive();
    }

    [Rpc(SendTo.Everyone)]
    private void OnAllQuestsCompletedRpc()
    {
        OnGlobalQuestCompleted?.Invoke(this, EventArgs.Empty);
    }

    private void SetNextQuestActive()
    {
        activeQuest.EnableCollider(false);
        buildingQuests.Remove(activeQuest);
        activeQuest.gameObject.SetActive(true);
    }
}
