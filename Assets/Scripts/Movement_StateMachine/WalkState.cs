using UnityEngine;
using UnityEngine.InputSystem;

public class WalkState : IMovementState
{
    public void DoState(PlayerMovement playerMovement)
    {
        playerMovement.currentSpeed = playerMovement.baseSpeed;
    }

    public void OnInputDown(PlayerMovement playerMovement, InputActionReference IAReference)
    {
        if (IAReference.action.name == "Run")
            playerMovement.ChangeState?.Invoke(new RunState());
        if (IAReference.action.name == "Jump")
            playerMovement.ChangeState?.Invoke(new JumpState());
    }

    public void OnInputUp(PlayerMovement playerMovement, InputActionReference IAReference)
    {
        if (IAReference.action.name == "Movement")
            playerMovement.ChangeState?.Invoke(new IdleState());
    }

    public void OnCollide(PlayerMovement playerMovement, Collider collider)
    {
        //
    }
}
