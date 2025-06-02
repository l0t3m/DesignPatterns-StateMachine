using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Physics")]
    public Rigidbody rb;

    [Header("Movement Settings")]
    [NonSerialized] public float baseSpeed = 18f;
    [NonSerialized] public float currentSpeed;
    public float acceleration = 18f;
    public float deceleration = 25f;

    [Header("Jump Settings")]
    public float jumpForce = 60f;
    public float doubleJumpForceMultiplier = 1.3f;
    public float fallGravityMultiplier = 18f;
    public float doubleJumpCooldown = 0.15f;

    [Header("Input Actions")]
    public InputActionReference moveAction;
    public InputActionReference runAction;
    public InputActionReference jumpAction;

    
    private float jumpCooldownTimer = 0f;
    private Vector2 moveDirection;

    // State Management
    private IMovementState currentState = new IdleState();
    public Action<IMovementState> ChangeState;

    private void Start()
    {
        ChangeState += OnChangeState;
        currentSpeed = baseSpeed;

        moveAction.action.Enable();
        runAction.action.Enable();
        jumpAction.action.Enable();

        BindCurrentAction(true);
    }

    private void Update()
    {
        moveDirection = moveAction.action.ReadValue<Vector2>();
        jumpCooldownTimer += Time.deltaTime;

        if (moveDirection == Vector2.zero && Mathf.Approximately(rb.linearVelocity.y, 0f))
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.linearVelocity = Vector3.Lerp(flatVel, Vector3.zero, deceleration * Time.deltaTime);
        }

        if (currentState is JumpState || currentState is DoubleJumpState)
        {
            if (rb.linearVelocity.y < 0)
            {
                rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallGravityMultiplier - 1) * Time.deltaTime;
            }
        }
    }

    private void FixedUpdate()
    {
        if (currentState is not IdleState)
            MovePlayer();
    }

    private void MovePlayer()
    {
        Vector3 force = new Vector3(moveDirection.x, 0f, moveDirection.y) * currentSpeed * acceleration;
        rb.AddForce(force, ForceMode.Acceleration);

        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (flatVelocity.magnitude > currentSpeed)
        {
            Vector3 limitedVel = flatVelocity.normalized * currentSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    public void DoJump()
    {
        jumpCooldownTimer = 0;

        float force = jumpForce;
        if (currentState is DoubleJumpState)
            force *= doubleJumpForceMultiplier;

        rb.AddForce(Vector3.up * force, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (runAction.action.IsPressed())
            ChangeState?.Invoke(new RunState());
        else if (moveAction.action.IsPressed())
            ChangeState?.Invoke(new WalkState());

        currentState.OnCollide(this, collision.collider);
    }

    public void OnChangeState(IMovementState newState)
    {
        BindCurrentAction(false);
        currentState = newState;
        newState.DoState(this);
        BindCurrentAction(true);
    }

    public void BindCurrentAction(bool bind)
    {
        if (bind)
        {
            moveAction.action.performed += ctx => currentState.OnInputDown(this, moveAction);
            moveAction.action.canceled += ctx => currentState.OnInputUp(this, moveAction);
            runAction.action.performed += ctx => currentState.OnInputDown(this, runAction);
            runAction.action.canceled += ctx => currentState.OnInputUp(this, runAction);
            jumpAction.action.performed += HandleJumpPerformed;
        }
        else
        {
            moveAction.action.performed -= ctx => currentState.OnInputDown(this, moveAction);
            moveAction.action.canceled -= ctx => currentState.OnInputUp(this, moveAction);
            runAction.action.performed -= ctx => currentState.OnInputDown(this, runAction);
            runAction.action.canceled -= ctx => currentState.OnInputUp(this, runAction);
            jumpAction.action.performed -= HandleJumpPerformed;
        }
    }

    private void HandleJumpPerformed(InputAction.CallbackContext ctx)
    {
        if (jumpCooldownTimer >= doubleJumpCooldown)
            currentState.OnInputDown(this, jumpAction);
    }
}
