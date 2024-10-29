using Sirenix.OdinInspector;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour
{
    [SerializeField]
    private float maxSpeed;
    [SerializeField]
    private float gravity;
    private CharacterController controller;
    [SerializeField]
    private LayerMask groundMask;
    [SerializeField]
    private float groundOffset;
    [SerializeField]
    private float groundRadius;
    [SerializeField]
    private float jumpHeight;
    [SerializeField]
    private float maxVerticalVelocity;
    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private Animator animator;

    private bool isGrounded;
    private float verticalVelocity;
    private Vector2 inputDirection;
    public bool CanMove = false;
    private Transform mainCameraTransform;

    private float currentSpeed;
    [SerializeField]
    private float acceleration;

    private void Start()
    {
        mainCameraTransform = Camera.main.transform;
        controller = GetComponent<CharacterController>();
        currentSpeed = 0f;
    }

    private void Update()
    {
        if (!IsOwner || !IsSpawned) return;

        CheckGround();
        HandleGravity();
        HandleAcceleration();
        Move();
        RotateTowardsPlayer();

        animator.SetBool("IsGrounded", isGrounded);
        if (Input.GetKeyDown(KeyCode.Q))
        {
            bool value = animator.GetBool("IsCarrying");
            animator.SetBool("IsCarrying", !value);
        }
    }

    private void HandleAcceleration()
    {
        float accelerationPerFrame = acceleration * Time.deltaTime;
        accelerationPerFrame = inputDirection != Vector2.zero ? accelerationPerFrame : accelerationPerFrame * (-1);
        currentSpeed += accelerationPerFrame;
        currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);
    }

    private void Move()
    {
        Vector3 directionVector = GetMoveDirection();
        Vector3 moveVector = directionVector * currentSpeed * Time.deltaTime;
        Vector3 gravityVector = new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime;
        controller.Move(moveVector + gravityVector);

        float animatorSpeedValue = currentSpeed / maxSpeed;
        animator.SetFloat("Speed", animatorSpeedValue);
    }

    private void CheckGround()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundOffset, transform.position.z);
        isGrounded = Physics.CheckSphere(spherePosition, groundRadius, groundMask, QueryTriggerInteraction.Ignore);
    }

    private void HandleGravity()
    {
        if (!isGrounded)
        {
            if (verticalVelocity < maxVerticalVelocity)
            {
                verticalVelocity += gravity * Time.deltaTime;
            }
        }
    }

    public virtual void RotateTowardsPlayer()
    {
        if (inputDirection == Vector2.zero)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(GetMoveDirection(), Vector2.up);
        Quaternion currentRotation = transform.rotation;
        Quaternion newRotation = Quaternion.Lerp(currentRotation, targetRotation, Time.deltaTime * rotationSpeed);
        transform.rotation = newRotation;
    }

    private Vector3 GetMoveDirection()
    {
        Vector3 cameraRight = mainCameraTransform.right;
        cameraRight.y = 0;
        Vector3 cameraForward = mainCameraTransform.forward;
        cameraForward.y = 0;

        Vector3 rightMovement = cameraRight.normalized * inputDirection.x;
        Vector3 forwardMovement = cameraForward.normalized * inputDirection.y;
        Vector3 direction = (rightMovement + forwardMovement).normalized;
        return direction;
    }

    #region InputEvents

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!IsOwner || !IsSpawned)
            return;

        if (!CanMove)
        {
            inputDirection = Vector2.zero;
            return;
        }
        inputDirection = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!IsOwner || !IsSpawned)
            return;

        if (!context.started)
            return;

        if (!CanMove)
            return;

        if (isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump");
        }
    }

    #endregion

    
}
    