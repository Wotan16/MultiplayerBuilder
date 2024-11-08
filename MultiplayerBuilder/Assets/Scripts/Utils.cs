using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static List<Transform> GetChildrenOfTransform(Transform transform)
    {
        List<Transform> children = new List<Transform>();
        foreach (Transform child in transform)
        {
            children.Add(child);
        }
        return children;
    }

    public static List<Transform> GetAllChildrenOfTransform(Transform transform)
    {
        List<Transform> allChildren = GetChildrenOfTransform(transform);
        for (int i = 0; i < allChildren.Count; i++)
        {
            List<Transform> childrenOfchild = GetChildrenOfTransform(allChildren[i]);
            allChildren.AddRange(childrenOfchild);
        }
        return allChildren;
    }
}
