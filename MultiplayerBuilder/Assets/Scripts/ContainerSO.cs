using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ContainerSO", menuName = "Scriptable Objects/ContainerSO")]
public class ContainerSO : ScriptableObject
{
    public Container prefab;
    public string containerName;
    public List<ResourceSO> containableResources;
    public bool isDisposable;
    [Tooltip("Will be filled with first resource in Containable Resources if enabled")]
    public bool filledByDefault;
}
