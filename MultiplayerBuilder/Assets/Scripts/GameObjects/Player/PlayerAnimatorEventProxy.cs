using System;
using UnityEngine;

public class PlayerAnimatorEventProxy : MonoBehaviour
{
    public event Action OnFootstepHappend;

    //animator events
    public void PlayFootstepsSound_Anim()
    {
        OnFootstepHappend?.Invoke();
    }

    public void LandedOnFeet_Anim()
    {
        OnFootstepHappend?.Invoke();
    }
}
