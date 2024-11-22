using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    [SerializeField]
    private AudioSource source;
    [SerializeField]
    private Player player;
    [SerializeField]
    private PlayerAnimatorEventProxy eventProxy;

    [SerializeField]
    private List<AudioClip> footsteps;
    private int index = 0;

    private void Start()
    {
        eventProxy.OnFootstepHappend += PlayFootstepsSound_Anim;
    }

    private void PlayFootstepsSound_Anim()
    {
        if (player.IsDead || !player.IsGrounded)
            return;

        source.PlayOneShot(footsteps[index]);

        index++;
        if (index >= footsteps.Count)
        {
            index = 0;
        }
    }
}
