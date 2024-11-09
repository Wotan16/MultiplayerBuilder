using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    public event Action OnTimerEnds;
    private float timeLeft;

    public Timer(float time)
    {
        timeLeft = time;

        UpdateProxy.OnUpdate += UpdateProxy_OnUpdate;
    }

    private void UpdateProxy_OnUpdate()
    {
        if(timeLeft <= 0)
        {
            OnTimerEnds?.Invoke();
            UpdateProxy.OnUpdate -= UpdateProxy_OnUpdate;
        }

        timeLeft -= Time.deltaTime;
    }

    public void Reset(float newTimeLeft)
    {
        timeLeft = newTimeLeft;
        UpdateProxy.OnUpdate += UpdateProxy_OnUpdate;
    }

    public void Stop()
    {
        UpdateProxy.OnUpdate -= UpdateProxy_OnUpdate;
        OnTimerEnds = null;
    }
}
