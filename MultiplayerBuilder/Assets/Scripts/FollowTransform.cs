using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    public Transform TargetTransform;
    private Rigidbody rb;
    private bool hasRB {  get { return rb != null; } }

    private void Awake()
    {
        if(!TryGetComponent(out Rigidbody rb))
            return;
        this.rb = rb;
    }

    private void FixedUpdate()
    {
        if (!hasRB)
            return;

        if (TargetTransform == null)
            return;

        rb.MovePosition(TargetTransform.position);
        rb.MoveRotation(TargetTransform.rotation);
    }

    private void LateUpdate()
    {
        if (hasRB)
            return;

        if (TargetTransform == null)
            return;

        transform.position = TargetTransform.position;
        transform.rotation = TargetTransform.rotation;
    }
}
