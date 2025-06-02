using System;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IMovementState
{
    void DoState(PlayerMovement playerMovement);
    void OnInputDown(PlayerMovement playerMovement, InputActionReference IAReference);
    void OnInputUp(PlayerMovement playerMovement, InputActionReference IAReference);
    void OnCollide(PlayerMovement playerMovement, Collider collider);
}
