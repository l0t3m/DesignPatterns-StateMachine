using UnityEngine;
using UnityEngine.InputSystem;

public class DoubleJumpState : IMovementState
{
    public void DoState(PlayerMovement playerMovement)
    {
        playerMovement.currentSpeed = playerMovement.baseSpeed;
        playerMovement.DoJump();
    }

    public void OnInputDown(PlayerMovement playerMovement, InputActionReference IAReference)
    {
        //
    }

    public void OnInputUp(PlayerMovement playerMovement, InputActionReference IAReference)
    {
        //
    }

    public void OnCollide(PlayerMovement playerMovement, Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            playerMovement.ChangeState?.Invoke(new IdleState());
        }
    }
}
