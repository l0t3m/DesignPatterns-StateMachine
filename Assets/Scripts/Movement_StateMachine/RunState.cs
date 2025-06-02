using UnityEngine;
using UnityEngine.InputSystem;

public class RunState : IMovementState
{
    public void DoState(PlayerMovement playerMovement)
    {
        playerMovement.currentSpeed = playerMovement.baseSpeed * 1.5f;
    }

    public void OnInputDown(PlayerMovement playerMovement, InputActionReference IAReference)
    {
        if (IAReference.action.name == "Jump")
            playerMovement.ChangeState?.Invoke(new JumpState());
    }

    public void OnInputUp(PlayerMovement playerMovement, InputActionReference IAReference)
    {
        if (IAReference.action.name == "Movement")
            playerMovement.ChangeState?.Invoke(new IdleState());
        else if (IAReference.action.name == "Run")
            playerMovement.ChangeState?.Invoke(new WalkState());
    }

    public void OnCollide(PlayerMovement playerMovement, Collider collider)
    {
        //
    }
}