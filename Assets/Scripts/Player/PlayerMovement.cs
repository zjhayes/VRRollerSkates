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
        Vector3 direction = CalculateForwardDirectionFromInput(controllerMotionInput.normalized);
        float magnitude = controllerMotionInput.magnitude;
        Vector3 velocity = direction * magnitude;

        // Transform the final velocity from local space to global space.
        Vector3 localVelocity = transform.TransformDirection(velocity);

        // Add to momentum.
        physics.Momentum += localVelocity;

        // Rotate character based on X movement.
        float rotationInput = -controllerMotionInput.x;
        physics.AngularMomentum += rotationInput * rotationSpeed * Time.deltaTime;
    }

    private Vector3 CalculateForwardDirectionFromInput(Vector3 input)
    {
        // Base direction on combined Y and Z axis motion.
        Vector3 direction = new(0f, 0f, Mathf.Abs(input.z) + Mathf.Abs(input.y));
        direction.Normalize();

        return direction;
    }

    private void OnDisable()
    {
        physics.OnUpdateMomentum -= HandleMovementFromHandMotion;
    }
}