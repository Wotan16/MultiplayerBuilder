using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PickupListSO", menuName = "Scriptable Objects/PickupListSO")]
public class PickupListSO : ScriptableObject
{
    public List<PickupSO> list;
}
