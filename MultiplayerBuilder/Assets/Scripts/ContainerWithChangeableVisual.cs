using UnityEngine;

public class ContainerWithChangeableVisual : Container
{
    [SerializeField]
    private ContainerVisual visual;
    
    protected override void Awake()
    {
        base.Awake();
        OnContainedResourceSOChanged += BucketContainer_OnContainedResourceSOChanged;
    }

    private void BucketContainer_OnContainedResourceSOChanged(object sender, System.EventArgs e)
    {
        visual.SetResourceVisual(ContainedResorceSO);
    }
}
