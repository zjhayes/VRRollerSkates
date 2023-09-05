using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerPhysics))]
[RequireComponent (typeof(GestureMoveProvider))]
public class PlayerMovement : GameBehaviour
{
    private PlayerPhysics physics;
    private GestureMoveProvider moveProvider;

    private void Start()
    {
        physics = GetComponent<PlayerPhysics>();
        moveProvider = GetComponent<GestureMoveProvider>();
        physics.OnUpdateMomentum += HandleMovementFromHandMotion;
    }

    public void HandleMovementFromHandMotion()
    {
        List<Vector3> controllerVelocities = moveProvider.GetControllerVelocities();
        Vector3 controllerMotionInput = VectorService.CalculateAverageVector(controllerVelocities);

        // Move character forward.
        Vector3 direction = moveProvider.CalculateForwardDirectionFromInput(controllerMotionInput);
        float magnitude = moveProvider.CalculateForwardMagnitudeFromInput(controllerMotionInput);
        Vector3 velocity = direction * magnitude;

        // Transform the final velocity from local space to global space.
        Vector3 localVelocity = transform.TransformDirection(velocity);

        // Add to momentum.
        physics.Momentum += localVelocity;

        // Rotate character.
        float rotationInput = moveProvider.CalculateRotationFromInput(controllerMotionInput);
        physics.AngularMomentum += rotationInput;
    }

    private void OnDisable()
    {
        physics.OnUpdateMomentum -= HandleMovementFromHandMotion;
    }
}