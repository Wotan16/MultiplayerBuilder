using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerHands : MonoBehaviour
{
    [SerializeField]
    private Rig playerHandsRig;
    [SerializeField]
    private FollowTransform leftHandFollow;
    [SerializeField]
    private FollowTransform rightHandFollow;

    public void EnableRig(ICarriable carriable)
    {
        playerHandsRig.weight = 1f;
        leftHandFollow.TargetTransform = carriable.GetLeftHandPoint();
        rightHandFollow.TargetTransform = carriable.GetRightHandPoint();
    }

    public void DisableRig()
    {
        playerHandsRig.weight = 0f;
        leftHandFollow.TargetTransform = null;
        rightHandFollow.TargetTransform = null;
    }
}
