using Sirenix.OdinInspector;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class Player : NetworkBehaviour
{
    public static Player LocalInstance { get; private set; }

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

    private Container carriedContainer;
    public Container CarriedContainer { get { return carriedContainer; } }
    public bool HandsBusy { get { return carriedContainer != null; } }

    [SerializeField]
    private PlayerInteraction interaction;
    [SerializeField]
    private Transform carriedObjectParent;
    public Transform CarriedObjectParent { get { return carriedObjectParent; } }

    [SerializeField]
    private PlayerHands hands;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        currentSpeed = 0f;
    }

    private void Start()
    {
        mainCameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        if (!IsOwner || !IsSpawned) return;

        CheckGround();
        HandleGravity();
        HandleAcceleration();
        Move();
        RotateTowardMovement();

        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsCarrying", HandsBusy);

        if (Input.GetKeyDown(KeyCode.Q))
        {
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
            LocalInstance = this;

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        if(clientId == OwnerClientId && HandsBusy)
        {
            DropItem();
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

    public virtual void RotateTowardMovement()
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

    public void DropItem()
    {
        if (HandsBusy)
        {
            carriedContainer.OnDrop();
            carriedContainer = null;
        }
        hands.DisableRig();
        DisableHandsRigServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DisableHandsRigServerRpc(ulong senderClientId)
    {
        DisableHandsRigClientRpc(senderClientId);
    }

    [ClientRpc(RequireOwnership = false)]
    private void DisableHandsRigClientRpc(ulong senderClientId)
    {
        hands.DisableRig();
    }

    public void PickUpItem(Container container)
    {
        carriedContainer = container;
        hands.EnableRig(container);
    }

    public void OnInteract()
    {
        if (interaction.SelectedInteractable != null)
        {
            interaction.SelectedInteractable.OnInteract(this);
        }
    }

    public void OnInteractAlternative()
    {
        DropItem();
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

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!IsOwner || !IsSpawned)
            return;

        if (!context.started)
            return;

        OnInteract();
    }

    public void OnInteractAlternative(InputAction.CallbackContext context)
    {
        if (!IsOwner || !IsSpawned)
            return;

        if (!context.started)
            return;

        OnInteractAlternative();
    }

    #endregion


}
    