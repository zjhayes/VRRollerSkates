using UnityEngine;
using UnityEngine.XR;


public class CharacterMovement : MonoBehaviour
{
    [SerializeField]
    private CharacterController controller;
    [SerializeField]
    Transform headTransform;
    [SerializeField]
    private XRNodeManager nodeManager;
    [SerializeField]
    private float boostThreshold = 1f;
    [SerializeField]
    private float maximumVelocity = 10f;
    [SerializeField]
    private float momentumDecayTime = 1.0f;
    [SerializeField]
    private float gravity = 9.81f;

    private Vector3 momentum;
    private float stopVelocity = 0.001f; // The minimum velocity threshold before stopped.

    private void Start()
    {
        momentum = Vector3.zero;
    }

    private void Update()
    {
        // Calculate forward motion
        //Vector3 boostVelocity = CalculateVelocityFromHandMotion();

        //momentum += boostVelocity;

        // Apply acceleration from motion.
        momentum += CalculateAccelerationFromHandMotion();

        // Apply gravity
        momentum.y -= gravity * Time.deltaTime;

        // Cast a ray to detect ground and calculate the ground normal
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

        if (momentum.magnitude > 0f)
        {
            // Apply smoothed momentum decay.
            momentum = Vector3.Lerp(momentum, Vector3.zero, Time.deltaTime / momentumDecayTime);
        }
        
        if (momentum.magnitude > maximumVelocity)
        {
            // Clamp to maximum velocity.
            momentum = momentum.normalized * maximumVelocity;
        }
        else if(momentum.magnitude < stopVelocity)
        {
            momentum = Vector3.zero;
        }

        controller.Move(momentum * Time.deltaTime);
    }

    private Vector3 CalculateAccelerationFromHandMotion()
    {
        Vector3 velocity = Vector3.zero;

        velocity += CalculateNodeVelocity(XRNode.LeftHand);
        velocity += CalculateNodeVelocity(XRNode.RightHand);

        if (velocity.magnitude > boostThreshold)
        {
            return velocity;
        }

        return Vector3.zero;
    }

    private Vector3 CalculateNodeVelocity(XRNode node)
    {
        Vector3 velocity = Vector3.zero;

        if (nodeManager.TryGetNodeVelocity(node, out Vector3 nodeVelocity) && nodeManager.TryGetNodeForward(node, out Vector3 nodeForward))
        {
            // Get the normalized direction from the velocity vector
            Vector3 nodePosition = nodeManager.TryGetNodePosition(node, out nodePosition) ? nodePosition : Vector3.zero;

            Debug.DrawRay(headTransform.position, nodeForward, Color.green); // Draw a ray from headTransform position in the direction of nodeForward

            // Return the magnitude of the nodeVelocity vector in its direction
            velocity = nodeForward * nodeVelocity.magnitude;// headTransform.forward * nodeVelocity.magnitude;//nodeForward * nodeVelocity.magnitude;
        }

        return velocity;
    }

    private Vector3 CalculateAverageControllerDirection()
    {
        Vector3 averageControllerDirection = Vector3.zero;

        if(nodeManager.TryGetNodeForward(XRNode.LeftHand, out Vector3 leftControllerForward) &&
            nodeManager.TryGetNodeForward(XRNode.RightHand, out Vector3 rightControllerForward))
        {
            // Return average direction.
            averageControllerDirection = (leftControllerForward + rightControllerForward).normalized;
        }

        return averageControllerDirection;
    }
}
