using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FollowPath : NetworkBehaviour
{
    [SerializeField]
    private float maxSpeed;
    public bool canMove;

    [SerializeField]
    private float acceleration;
    private float currentSpeed;
    private float maxSpeedStoppingDistance;

    [SerializeField]
    private List<Transform> path;

    private Transform targetPoint;
    private Timer timer;

    private void Start()
    {
        targetPoint = path[0];
        currentSpeed = 0;
        //maxSpeedStoppingDistance = ((maxSpeed * maxSpeed) / (acceleration * 2f)) * (-1);
    }

    private void Update()
    {
        if (!IsServer)
            return;

        if (!canMove)
            return;


        HandleAcceleration();
        HandleMovement();
    }

    private void HandleAcceleration()
    {
        float distanceToTarget = Vector3.Distance(transform.position, targetPoint.position);
        maxSpeedStoppingDistance = (maxSpeed * maxSpeed) / (acceleration * 2f);
        
        float accelerationPerFrame = acceleration * Time.deltaTime;
        accelerationPerFrame = distanceToTarget > maxSpeedStoppingDistance ? accelerationPerFrame : accelerationPerFrame * (-1);

        currentSpeed += accelerationPerFrame;
        currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);

        if (currentSpeed <= 0f)
            WaitBeforeMove();
    }

    private void HandleMovement()
    {
        float moveAmount = currentSpeed * Time.deltaTime;
        Vector3 direction = targetPoint.position - transform.position;
        Vector3 moveVector = moveAmount * direction.normalized;
        transform.position += moveVector;
    }

    private void WaitBeforeMove()
    {
        canMove = false;
        timer = new Timer(1f);
        timer.OnTimerEnds += () =>
        {
            SetNextTargetPoint();
            canMove = true;
        };
    }

    //private void FixedUpdate()
    //{
    //    if (!IsServer)
    //        return;

    //    if (!canMove)
    //        return;

    //    float moveAmount = moveSpeed * Time.fixedDeltaTime;
    //    float distanceToTarget = Vector3.Distance(transform.position, targetPoint.position);
    //    bool willGetToTarget = moveAmount > distanceToTarget;
    //    moveAmount = willGetToTarget ? distanceToTarget : moveAmount;

    //    Vector3 direction = targetPoint.position - transform.position;
    //    Vector3 moveVector = moveAmount * direction.normalized;
    //    rb.MovePosition(rb.position + moveVector);

    //    if (willGetToTarget)
    //        SetNextTargetPoint();
    //}

    public void SetNextTargetPoint()
    {
        int index = path.IndexOf(targetPoint);
        int nextPointIndex = index + 1 >= path.Count ? 0 : index + 1;
        targetPoint = path[nextPointIndex];
    }

    private void OnDrawGizmos()
    {
        if (path == null)
            return;

        if (path.Count < 2)
            return;

        foreach (Transform t in path)
        {
            if (t == null)
                return;
        }

        Gizmos.color = Color.cyan;
        for (int i = 0; i < path.Count; i++)
        {
            int nextPointIndex = i+1 >= path.Count ? 0 : i + 1;
            Gizmos.DrawLine(path[i].position, path[nextPointIndex].position);
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if(timer != null)
            timer.Stop();
    }
}
