using System;
using Unity.Netcode;
using UnityEngine;

public class MyTransformSync : NetworkBehaviour
{
    private Rigidbody rb;
    private bool hasRB { get { return rb != null; } }
    public NetworkVariable<bool> enableSync = new NetworkVariable<bool>();
    [SerializeField]
    private float timeToLerp = 0.3f;
    private float timeToLerpDelta;
    private bool isLerping { get { return timeToLerpDelta > 0f; } }
    [SerializeField]
    private float lerpModifier = 5f;
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    private void Awake()
    {
        if (!TryGetComponent(out Rigidbody rb))
            return;
        this.rb = rb;

        enableSync.OnValueChanged += EnableSync_OnValueChanged;
    }

    private void EnableSync_OnValueChanged(bool previousValue, bool newValue)
    {
        timeToLerpDelta = timeToLerp;
    }

    [ClientRpc()]
    private void UpdateTransformPositionClientRpc(Vector3 position, Quaternion rotation)
    {
        targetPosition = position;
        targetRotation = rotation;
    }

    private void FixedUpdate()
    {
        if (!enableSync.Value)
            return;

        if (IsServer)
        {
            if (!hasRB)
                return;

            UpdateTransformPositionClientRpc(transform.position, transform.rotation);
        }
        else
        {
            if (!hasRB)
                return;

            if (isLerping)
            {
                Vector3 newPosition = Vector3.Lerp(transform.position, targetPosition, lerpModifier * Time.fixedDeltaTime);
                Quaternion newRotation = Quaternion.Lerp(transform.rotation, targetRotation, lerpModifier * Time.fixedDeltaTime);
                rb.MovePosition(newPosition);
                rb.MoveRotation(newRotation);
            }
            else
            {
                rb.MovePosition(targetPosition);
                rb.MoveRotation(targetRotation);
            }
        }
    }

    private void LateUpdate()
    {
        if (!enableSync.Value)
            return;

        if (IsServer)
        {
            if (hasRB)
                return;

            UpdateTransformPositionClientRpc(transform.position, transform.rotation);
        }
        else
        {
            if (isLerping)
            {
                timeToLerpDelta -= Time.deltaTime;

                if (!hasRB)
                {
                    Vector3 newPosition = Vector3.Lerp(transform.position, targetPosition, lerpModifier * Time.deltaTime);
                    Quaternion newRotation = Quaternion.Lerp(transform.rotation, targetRotation, lerpModifier * Time.deltaTime);
                    transform.position = newPosition;
                    transform.rotation = newRotation;
                }
            }
            else
            {
                if (!hasRB)
                {
                    transform.position = targetPosition;
                    transform.rotation = targetRotation;
                }
            }
        }
    }
}
