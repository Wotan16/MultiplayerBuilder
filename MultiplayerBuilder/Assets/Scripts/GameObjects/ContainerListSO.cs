using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "ContainerListSO", menuName = "Scriptable Objects/ContainerListSO")]
public class ContainerListSO : ScriptableObject
{
    public List<ContainerSO> list;
}
