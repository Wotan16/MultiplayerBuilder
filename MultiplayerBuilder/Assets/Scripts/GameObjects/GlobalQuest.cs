using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GlobalQuest : NetworkBehaviour
{
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
            Debug.Log("Peremoga");
            //PEREMOGA
            return;
        }

        OnQuestCompleteClientRpc();
    }

    [ClientRpc]
    private void OnQuestCompleteClientRpc()
    {
        SetNextQuestActive();
    }

    private void SetNextQuestActive()
    {
        activeQuest.EnableCollider(false);
        buildingQuests.Remove(activeQuest);
        activeQuest.gameObject.SetActive(true);
    }
}
