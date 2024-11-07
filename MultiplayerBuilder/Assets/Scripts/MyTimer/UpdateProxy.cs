using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateProxy : MonoBehaviour
{
    public static UpdateProxy Instance;
    
    public static event Action OnUpdate;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("More than one " + GetType().Name);
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        OnUpdate?.Invoke();
    }
}
