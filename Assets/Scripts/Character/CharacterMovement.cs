using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;


public class CharacterMovement : GameBehaviour
{
    [SerializeField]
    private CharacterController controller;
    [SerializeField]
    Transform headTransform;
    [SerializeField]
    private float boostThreshold = 1f;
    [SerializeField]
    private float maximumVelocity = 10f;
    [SerializeField]
    private float momentumDecayTime = 1.0f;
    [SerializeField]
    private float gravity = 9.81f;

    private Vector3 momentum;
    private readonly float stopVelocity = 0.001f; // The velocity threshold considered stopped.

    private void Start()
    {
        momentum = Vector3.zero;
    }

    private void Update()
    {
        HandleMovementFromHandMotion();

        ApplyGravity();

        ApplySlope();

        ApplyMomentumDecay();

        ClampMomentum();

        ApplyMomentumToMovement();
    }

    private void ApplyMomentumToMovement()
    {
        Debug.Log(momentum);
        controller.Move(momentum * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        momentum.y -= gravity * Time.deltaTime;
    }

    private void ApplyMomentumDecay()
    {
        if (momentum.magnitude > 0f)
        {
            // Apply smoothed momentum decay.
            momentum = Vector3.Lerp(momentum, Vector3.zero, Time.deltaTime / momentumDecayTime);
        }
    }

    private void ClampMomentum()
    {

        if (momentum.magnitude > maximumVelocity)
        {
            // Clamp to maximum velocity.
            momentum = momentum.normalized * maximumVelocity;
        }
        else if (momentum.magnitude < stopVelocity)
        {
            momentum = Vector3.zero;
        }
    }

    private void ApplySlope()
    {
        // Cast a ray to detect ground and calculate the ground normal.
        RaycastHit hit;
        Vector3 groundNormal = Vector3.up; // Default to upright
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, controller.height / 2 + 0.1f))
        {
            groundNormal = hit.normal;
        }

        // Calculate the slope angle
        float slopeAngle = Vector3.Angle(Vector3.up, groundNormal);

        if (slopeAngle > 0f)
        {
            // Calculate a speed multiplier based on the slope angle
            float slopeSpeedMultiplier = Mathf.Lerp(1f, 1.5f, slopeAngle / 45f);

            // Calculate the slope-adjusted momentum direction
            Vector3 slopeAdjustedMomentum = Vector3.ProjectOnPlane(momentum, groundNormal) * slopeSpeedMultiplier;

            // Blend the slope-adjusted momentum with the original momentum
            float blendFactor = 0.5f;
            momentum = Vector3.Lerp(momentum, slopeAdjustedMomentum, blendFactor); // Adjust blending factor as needed
        }
    }

    private void HandleMovementFromHandMotion()
    {
        List<Vector3> controllerVelocities = GetControllerVelocities();
        Vector3 controllerMotionInput = VectorService.CalculateAverageVector(controllerVelocities);

        // Move character forward.
        Vector3 direction = CalculateForwardDirectionFromInput(controllerMotionInput);
        float magnitude = CalculateForwardMagnitudeFromInput(controllerMotionInput);
        // Calculate the final velocity by multiplying the direction by the magnitude.
        Vector3 velocity = direction * magnitude;

        // Transform the final velocity from local space to global space.
        Vector3 localVelocity = transform.TransformDirection(velocity);

        // Add to momentum.
        momentum += localVelocity;

        // Rotate character.
        float rotationInput = CalculateRotationFromInput(controllerMotionInput);
        RotateCharacter(rotationInput);
    }

    private List<Vector3> GetControllerVelocities()
    {
        List<Vector3> controllerVelocities = new List<Vector3>();

        if (TryCalculateControllerVelocity(XRNode.LeftHand, out Vector3 leftHandVelocity))
        {
            controllerVelocities.Add(leftHandVelocity);
        }

        if (TryCalculateControllerVelocity(XRNode.RightHand, out Vector3 rightHandVelocity))
        {
            controllerVelocities.Add(rightHandVelocity);
        }

        return controllerVelocities;
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

    private float CalculateForwardMagnitudeFromInput(Vector3 input)
    {
        return input.magnitude;
    }

    private bool TryCalculateControllerVelocity(XRNode node, out Vector3 handVelocity)
    {
        if (gameManager.XR.NodeManager.TryGetNodeVelocity(node, out handVelocity) && handVelocity.magnitude > boostThreshold)
        {
            return true;
        }
        else return false;
    }

    private float CalculateRotationFromInput(Vector3 input)
    {
        Vector3 direction = input.normalized;

        // Extract the X-axis motion for rotation.
        return direction.x;
    }

    private void RotateCharacter(float rotationInput)
    {
        // You can adjust the rotation speed based on the input.
        float rotationSpeed = 45f; // Adjust as needed.

        // Calculate the rotation angle based on input and time.
        float rotationAngle = rotationInput * rotationSpeed * Time.deltaTime;
        
        // Rotate the character around the Y-axis.
        transform.Rotate(Vector3.up, rotationAngle);
    }
}