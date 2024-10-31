using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipeSO", menuName = "Scriptable Objects/RecipeSO")]
public class RecipeSO : ScriptableObject
{
    public List<PickupSO> recipeList;
    public float timeToMake;
}
