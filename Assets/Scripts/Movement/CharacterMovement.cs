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
    private float momentumDecay = 0.99f;
    [SerializeField]
    private float gravity = 9.81f;

    private Vector3 momentum;

    private void Start()
    {
        momentum = Vector3.zero;
    }

    private void Update()
    {
        // Calculate forward motion
        Vector3 boostVelocity = CalculateVelocityFromHandMotion();

        momentum += boostVelocity;

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

            // Apply the speed multiplier to the move direction
            momentum = Vector3.ProjectOnPlane(momentum, groundNormal) * slopeSpeedMultiplier;
        }
        else
        {
            momentum *= momentumDecay;
        }

        // Clamp to maximum velocity.
        if (momentum.magnitude > maximumVelocity)
        {
            momentum = momentum.normalized * maximumVelocity;
        }

        controller.Move(momentum * Time.deltaTime);
    }

    private Vector3 CalculateVelocityFromHandMotion()
    {
        Vector3 velocity = Vector3.zero;

        velocity += CalculateNodeVelocity(XRNode.LeftHand);
        velocity += CalculateNodeVelocity(XRNode.RightHand);

        return velocity;
    }

    private Vector3 CalculateNodeVelocity(XRNode node)
    {
        Vector3 velocity = Vector3.zero;
        if (TryGetNodeVelocity(node, out Vector3 nodeVelocity))
        {
            if (nodeVelocity.magnitude > boostThreshold)
            {
                return headTransform.forward * nodeVelocity.magnitude;
            }
        }
        return velocity;
    }

    private bool TryGetNodeVelocity(XRNode nodeType, out Vector3 velocity)
    {
        velocity = Vector3.zero;
        if (nodeManager.TryGetNodeState(nodeType, out XRNodeState nodeState) &&
            nodeState.TryGetVelocity(out velocity))
        {
            // Node velocity found.
            return true;
        }
        return false; // Couldn't get velocity.
    }
}
