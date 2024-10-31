using NUnit.Framework;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ConcreteMixer : BaseCraftingStation
{
    [SerializeField]
    private ConcreteMixerVisual visual;

    protected override void Awake()
    {
        base.Awake();
        visual.DisableOutline();
    }

    public override void OnSelected()
    {
        visual.EnableOutline();
    }

    public override void OnDeselected()
    {
        visual.DisableOutline();
    }


    [ClientRpc]
    private void EndedMixingClientRpc()
    {
        Debug.Log("Mixed");
    }

    protected override void OnCraftingEnded()
    {
        EndedMixingClientRpc();
    }
}
