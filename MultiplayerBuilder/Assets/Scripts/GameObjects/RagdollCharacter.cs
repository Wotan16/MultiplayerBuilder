using System.Collections.Generic;
using UnityEngine;

public class RagdollCharacter : MonoBehaviour
{
    [SerializeField]
    private float timeToDestroy;
    private float timeToDestroyDelta;
    [SerializeField]
    private Transform root;

    private List<Rigidbody> ragdollBodyParts;

    private void Awake()
    {
        timeToDestroyDelta = timeToDestroy;
        InitializeRigidbodies();
    }

    private void InitializeRigidbodies()
    {
        Rigidbody[] childRigidbodies = GetComponentsInChildren<Rigidbody>();
        ragdollBodyParts = new List<Rigidbody>();

        foreach (Rigidbody child in childRigidbodies)
        {
            if (!child.CompareTag("BodyPart"))
                continue;
            ragdollBodyParts.Add(child);
            child.isKinematic = true;
        }
    }

    private void Update()
    {
        if(timeToDestroy <= 0f)
        {
            Destroy(gameObject);
            return;
        }
        timeToDestroyDelta -= Time.deltaTime;
    }

    public void MatchWithHumanoidSkeleton(Transform rootBone)
    {
        MatchBoneWithAllChildren(root, rootBone);
    }
    
    private void MatchBoneWithAllChildren(Transform bone, Transform boneToMatch)
    {
        MatchPositionAndRotation(bone, boneToMatch);
        //int minChildCount = bone.childCount < boneToMatch.childCount ? bone.childCount : boneToMatch.childCount;
        //for (int i = 0; i < minChildCount; i++)
        //{
        //    MatchBoneWithAllChildren(bone.GetChild(i), boneToMatch.GetChild(i));
        //}

        foreach(Transform child in bone)
        {
            foreach (Transform childToMatch in boneToMatch)
            {
                if(child.name == childToMatch.name)
                {
                    MatchBoneWithAllChildren(child, childToMatch);
                    break;
                }
            }
        }
    }

    private void MatchPositionAndRotation(Transform match, Transform matchFrom)
    {
        match.position = matchFrom.position;
        match.rotation = matchFrom.rotation;
    }

    public void TurnOnRagdoll()
    {
        foreach (Rigidbody rb in ragdollBodyParts)
        {
            rb.isKinematic = false;
        }
    }

    public void TurnOffRagdoll()
    {
        foreach (Rigidbody rb in ragdollBodyParts)
        {
            rb.isKinematic = true;
        }
    }

    public void PushRagdoll(Vector3 direction, float force, Collider collider)
    {
        Rigidbody rb = collider.attachedRigidbody;
        rb.AddForceAtPosition(direction * force, collider.transform.position, ForceMode.Impulse);
    }
}
