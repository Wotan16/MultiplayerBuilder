using Unity.Netcode.Components;
using UnityEngine;

public class MyClientNetworkTransform : NetworkTransform
{

    [SerializeField]
    private float timeBeforeInterpolate;
    private float timeBeforeInterpolateDelta;

    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }

    private void Update()
    {
        if (Interpolate)
            return;

        if(timeBeforeInterpolateDelta <= 0)
            Interpolate = true;
        else
            timeBeforeInterpolateDelta -= Time.deltaTime;
    }

    public void DisableInterpolationTeporarily(float time)
    {
        timeBeforeInterpolateDelta = time;
        Interpolate = false;
    }
}
