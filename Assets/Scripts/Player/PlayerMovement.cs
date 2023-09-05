using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerPhysics))]
[RequireComponent (typeof(XRGestureMoveProvider))]
public class PlayerMovement : GameBehaviour
{
    [SerializeField]
    private float rotationSpeed = 1.0f;

    private PlayerPhysics physics;
    private XRGestureMoveProvider moveProvider;

    private void Start()
    {
        physics = GetComponent<PlayerPhysics>();
        moveProvider = GetComponent<XRGestureMoveProvider>();
        physics.OnUpdateMomentum += HandleMovementFromHandMotion;
    }

    private void HandleMovementFromHandMotion()
    {
        List<Vector3> controllerVelocities = moveProvider.GetControllerVelocities();
        Vector3 controllerMotionInput = VectorService.CalculateAverageVector(controllerVelocities);

        // Move character forward.
        Vector3 direction = CalculateForwardDirectionFromInput(controllerMotionInput);
        float magnitude = controllerMotionInput.magnitude;
        Vector3 velocity = direction * magnitude;

        // Transform the final velocity from local space to global space.
        Vector3 localVelocity = transform.TransformDirection(velocity);

        // Add to momentum.
        physics.Momentum += localVelocity;

        // Rotate character.
        float rotationInput = controllerMotionInput.x;
        physics.AngularMomentum += rotationInput * rotationSpeed * Time.deltaTime;
    }

    private Vector3 CalculateForwardDirectionFromInput(Vector3 input)
    {
        // Normalize the total velocity to get the direction.
        Vector3 direction = input.normalized;

        // Calculate the forward motion by removing the X-axis contribution.
        direction.x = 0;

        // Ensure that the direction is relative to the character's forward direction.
        direction.z = direction.z + direction.y; // Upward and forward motion contributes to forward movement.
        direction.y = 0; // Zero out the vertical component.
        direction.Normalize();

        return direction;
    }

    private void OnDisable()
    {
        physics.OnUpdateMomentum -= HandleMovementFromHandMotion;
    }
}