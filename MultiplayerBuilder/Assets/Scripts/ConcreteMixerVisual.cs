using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ConcreteMixerVisual : MonoBehaviour
{
    [SerializeField]
    private List<Outline> outlines;

    public void EnableOutline()
    {
        foreach (Outline outline in outlines)
        {
            outline.enabled = true;
        }
    }

    public void DisableOutline()
    {
        foreach (Outline outline in outlines)
        {
            outline.enabled = false;
        }
    }
}
