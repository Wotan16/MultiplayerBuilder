using UnityEngine;
using Unity.Netcode;

public class Test : MonoBehaviour
{
    public GameObject characterObject;
    public Transform root;
    public RagdollCharacter characterPrefab;
    public RagdollCharacter ragdoll;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ragdoll = Instantiate(characterPrefab);
            ragdoll.MatchWithHumanoidSkeleton(root);
            characterObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            ragdoll.TurnOnRagdoll();
        }
    }
}
