using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ContainerVisual : MonoBehaviour
{
    [SerializeField]
    private Transform resourceVisualPoint;
    private Transform resourceVisual;

    [Serializable]
    private class ResourceSOVisualPair
    {
        public ResourceSO resourceSO;
        public GameObject visualObject;
    }

    [SerializeField]
    private List<ResourceSOVisualPair> resourceVisuals;

    private void Awake()
    {
        SetResourceVisual(null);
    }

    public void SetResourceVisual(ResourceSO resourceSO)
    {
        foreach (ResourceSOVisualPair resourceVisual in resourceVisuals)
        {
            if(resourceVisual.resourceSO == resourceSO)
                resourceVisual.visualObject.SetActive(true);
            else
                resourceVisual.visualObject.SetActive(false);
        }
    }
}
