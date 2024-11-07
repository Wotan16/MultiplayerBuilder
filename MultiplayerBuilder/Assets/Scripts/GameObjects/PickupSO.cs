using UnityEngine;

[CreateAssetMenu(fileName = "PickupSO", menuName = "Scriptable Objects/PickupSO")]
public class PickupSO : ScriptableObject
{
    public Pickup prefab;
    public string pickupName;
}
