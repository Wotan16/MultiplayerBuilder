using Unity.Netcode;
using UnityEngine;

public class ConcretePond : NetworkBehaviour
{
    [SerializeField]
    private QuestObject questObject;
    [SerializeField]
    private GameObject pobedaObject;
    [SerializeField]
    private RecipeSO peremogaRecipeSO;

    private void Start()
    {
        questObject.OnRecipeCompleted += QuestObject_OnRecipeCompleted;
        questObject.SetRecipe(peremogaRecipeSO);
    }

    private void QuestObject_OnRecipeCompleted(object sender, System.EventArgs e)
    {
        OnRecipeCompletedClientRpc();
    }

    [ClientRpc]
    private void OnRecipeCompletedClientRpc()
    {
        pobedaObject.SetActive(true);
    }
}
