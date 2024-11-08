using System;
using Unity.Netcode;
using UnityEngine;

public class BuildingQuest : NetworkBehaviour
{
    public event EventHandler OnQuestComplete_Server;

    [SerializeField]
    private QuestObject questObject;
    [SerializeField]
    private GameObject objectToBuild;
    [SerializeField]
    private GameObject completeObject;
    [SerializeField]
    private RecipeSO peremogaRecipeSO;

    private void Awake()
    {
        objectToBuild.SetActive(true);
        completeObject.SetActive(false);
        questObject.SetRecipe(peremogaRecipeSO);
    }

    private void Start()
    {
        if (IsServer)
        {
            questObject.OnRecipeCompleted_Server += QuestObject_OnRecipeCompleted;
        }
    }

    private void QuestObject_OnRecipeCompleted(object sender, System.EventArgs e)
    {
        OnRecipeCompletedClientRpc();
        OnQuestComplete_Server?.Invoke(this, EventArgs.Empty);
    }

    [ClientRpc]
    private void OnRecipeCompletedClientRpc()
    {
        objectToBuild.SetActive(false);
        completeObject.SetActive(true);
    }

    public void EnableCollider(bool value)
    {
        questObject.EnableCollider(value);
    }
}
