using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour
{
    public static Player LocalInstance { get; private set; }

    public static event EventHandler OnLocalInstanceSpawned;

    [SerializeField]
    private Animator animator;

    [Header("Movement")]
    [SerializeField]
    private float maxSpeed;
    [SerializeField]
    private float acceleration;
    [SerializeField]
    private float rotationSpeed;
    private float currentSpeed;
    private Transform mainCameraTransform;
    private CharacterController controller;
    public bool CanMove = false;

    [Header("Gravity & Jump")]
    [SerializeField]
    private float gravity;
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
    private bool isGrounded;
    private float verticalVelocity;
    private Vector2 inputDirection;

    [Header("Death and Respawn")]
    [SerializeField]
    private GameObject playerVisual;
    [SerializeField]
    private RagdollCharacter ragdollPrefab;
    [SerializeField]
    private Transform rootBone;
    private bool isDead;
    public bool IsDead { get { return isDead; } }
    private Timer reviveTimer;
    private GameObject ragdollObject;
    [SerializeField]
    private float deathValidationDelay = 0.1f;
    private Timer deathValidationTimer;
    [SerializeField]
    private float confirmDeathDelay = 0.1f;
    private Timer confirmDeathTimer;

    [Header("Interaction")]
    [SerializeField]
    private PlayerInteraction interaction;
    [SerializeField]
    private Transform carriedObjectParent;
    public Transform CarriedObjectParent { get { return carriedObjectParent; } }

    private Container carriedContainer;
    public Container CarriedContainer { get { return carriedContainer; } }
    public bool HandsBusy { get { return carriedContainer != null; } }
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
        if (!IsOwner || !IsSpawned)
            return;

        if(isDead)
            return;

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
        {
            LocalInstance = this;
            OnLocalInstanceSpawned?.Invoke(this, EventArgs.Empty);
        }

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
        DisableHandsRigRpc();
    }

    [Rpc(SendTo.Everyone)]
    private void DisableHandsRigRpc()
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

    public void StopMovement()
    {
        inputDirection = Vector2.zero;
    }

    public void KillPlayer()
    {
        KillPlayer(Vector3.zero);
    }

    public void KillPlayer(Vector3 force)
    {
        if (isDead)
            return;

        Die(force);

        if (IsOwner)
        {
            OnOwnerPlayerDeadRpc(force);
        }
        else
        {
            deathValidationTimer = new Timer(deathValidationDelay);
            deathValidationTimer.OnTimerEnds += () =>
            {
                RequestDeathValidationRpc(RpcTarget.Single(OwnerClientId, RpcTargetUse.Temp));
            };

        }
    }

    [Rpc(SendTo.NotOwner, RequireOwnership = false)]
    private void OnOwnerPlayerDeadRpc(Vector3 force)
    {
        if (isDead) 
        {
            deathValidationTimer.Stop();
            Debug.Log("death Validation Timer Stopped");
            return;
        }

        confirmDeathTimer = new Timer(confirmDeathDelay);
        confirmDeathTimer.OnTimerEnds += () =>
        {
            if (isDead)
                return;

            Die(force);
        };
    }

    [Rpc(SendTo.SpecifiedInParams, RequireOwnership = false)]
    private void RequestDeathValidationRpc(RpcParams rpcParams = default)
    {
        ValidateDeathRpc(isDead, RpcTarget.Single(rpcParams.Receive.SenderClientId, RpcTargetUse.Temp));
        Debug.Log("Request received, isDead: " + isDead);
    }

    [Rpc(SendTo.SpecifiedInParams, RequireOwnership = false)]
    private void ValidateDeathRpc(bool isDead, RpcParams rpcParams = default)
    {
        if (isDead)
            return;

        Revive(transform.position);
    }

    private void Die(Vector3 force)
    {
        if (IsOwner)
        {
            Debug.Log("Died");
            controller.enabled = false;
            DropItem();
            StopMovement();
        }
        isDead = true;
        playerVisual.SetActive(false);

        float timeToRevive = 5f;

        RagdollCharacter ragdoll = Instantiate(ragdollPrefab);
        ragdollObject = ragdoll.gameObject;
        ragdoll.MatchWithHumanoidSkeleton(rootBone);
        ragdoll.DestroyAfterTime(timeToRevive);
        ragdoll.TurnOnRagdoll();
        ragdoll.PushRagdoll(force);

        reviveTimer = new Timer(timeToRevive);
        reviveTimer.OnTimerEnds += () => { Revive(Vector3.zero); };
    }

    private void Revive(Vector3 position)
    {
        if (IsOwner)
        {
            transform.position = position;
            controller.enabled = true;
        }

        playerVisual.SetActive(true);
        isDead = false;

        StopTimers();

        if (ragdollObject != null)
            Destroy(ragdollObject);
    }

    public override void OnDestroy()
    {
        StopTimers();

        base.OnDestroy();
    }

    private void StopTimers()
    {
        if (reviveTimer != null)
            reviveTimer.Stop();

        if (deathValidationTimer != null)
            deathValidationTimer.Stop();

        if(confirmDeathTimer != null)
            confirmDeathTimer.Stop();
    }

    #region InputEvents

    public void OnMove_Input(InputAction.CallbackContext context)
    {
        if (!IsOwner || !IsSpawned)
            return;

        if (isDead)
            return;

        if (!CanMove)
        {
            inputDirection = Vector2.zero;
            return;
        }
        inputDirection = context.ReadValue<Vector2>();
    }

    public void OnJump_Input(InputAction.CallbackContext context)
    {
        if (!IsOwner || !IsSpawned)
            return;

        if (isDead)
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

    public void OnInteract_Input(InputAction.CallbackContext context)
    {
        if (!IsOwner || !IsSpawned)
            return;

        if (isDead)
            return;

        if (!context.started)
            return;

        OnInteract();
    }

    public void OnInteractAlternative_Input(InputAction.CallbackContext context)
    {
        if (!IsOwner || !IsSpawned)
            return;

        if (isDead)
            return;

        if (!context.started)
            return;

        OnInteractAlternative();
    }

    #endregion
}
    