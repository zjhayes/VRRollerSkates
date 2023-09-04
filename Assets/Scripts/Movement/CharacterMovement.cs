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
    private float stopVelocity = 0.001f; // The velocity threshold considered stopped.

    private void Start()
    {
        momentum = Vector3.zero;
    }

    private void Update()
    {
        // Apply acceleration from motion.
        momentum += CalculateAccelerationFromHandMotion();

        // Apply gravity.
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

        //velocity += CalculateNodeVelocity(XRNode.LeftHand);
        //velocity += CalculateNodeVelocity(XRNode.RightHand);

        velocity += CalculateControllerForwardInput();
        //velocity += CalculateControllerForwardInput();

        return velocity;
    }

    private Vector3 CalculateControllerForwardInput()
    {
        Vector3 averageVelocity = Vector3.zero;
        int contributingHands = 0;
        /*if(nodeManager.TryGetNodeVelocity(XRNode.LeftHand, out Vector3 leftHandVelocity))
        {
            Vector3 localLeftHandVelocity = transform.InverseTransformDirection(leftHandVelocity);
            averageVelocity += localLeftHandVelocity;
            contributingHands++;
        }*/

        if(TryCalculateHandInputVelocity(XRNode.LeftHand, out Vector3 leftHandVelocity))
        {
            averageVelocity += leftHandVelocity;
            contributingHands++;
        }

        if (TryCalculateHandInputVelocity(XRNode.RightHand, out Vector3 rightHandVelocity))
        {
            //Vector3 localRightHandVelocity = transform.InverseTransformDirection(rightHandVelocity);
            //averageVelocity += localRightHandVelocity;
            averageVelocity += rightHandVelocity;
            contributingHands++;
        }
        
        if(contributingHands > 0)
        {
            averageVelocity /= contributingHands;
            /*
            // Transform the average velocity from global space to local space.
            Vector3 localAverageVelocity = transform.InverseTransformDirection(averageVelocity);
            
            // Ensure that the direction is relative to the character's forward direction.
            localAverageVelocity.x = localAverageVelocity.x + localAverageVelocity.y;
            localAverageVelocity.y = 0; // Zero out the vertical component.
            localAverageVelocity.Normalize();

            return localAverageVelocity;*/

            // Calculate the magnitude of the total velocity.
            float magnitude = averageVelocity.magnitude;

            // Normalize the total velocity to get the direction.
            Vector3 direction = averageVelocity.normalized;

            // Extract the X-axis motion for rotation.
            float rotationInput = direction.x;

            // Calculate the forward motion by removing the X-axis contribution.
            direction.x = 0;

            // Ensure that the direction is relative to the character's forward direction.
            direction.z = direction.z + direction.y;
            direction.y = 0; // Zero out the vertical component.
            direction.Normalize();

            // Calculate the final velocity by multiplying the direction by the magnitude.
            Vector3 finalVelocity = direction * magnitude;

            // Transform the final velocity from local space to global space.
            finalVelocity = transform.TransformDirection(finalVelocity);

            // Rotate the character based on the X-axis motion.
            RotateCharacter(rotationInput);

            return finalVelocity;
        }

        return averageVelocity;
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

    private bool TryCalculateHandInputVelocity(XRNode node, out Vector3 handVelocity)
    {
        if (nodeManager.TryGetNodeVelocity(node, out handVelocity) && handVelocity.magnitude > boostThreshold)
        {
            //Vector3 localRightHandVelocity = transform.InverseTransformDirection(rightHandVelocity);
            //averageVelocity += localRightHandVelocity;
            return true;
        }
        else return false;
    }
    /*
    private Vector3 CalculateNodeVelocity(XRNode node)
    {
        Vector3 velocity = Vector3.zero;

        if (nodeManager.TryGetNodeVelocity(node, out Vector3 nodeVelocity) && nodeManager.TryGetNodeForward(node, out Vector3 nodeForward))
        {
            // Return the magnitude of the nodeVelocity vector in its direction
            velocity = nodeForward * nodeVelocity.magnitude;// headTransform.forward * nodeVelocity.magnitude;//nodeForward * nodeVelocity.magnitude;
        }

        return velocity;
    }*/

    private Vector3 CalculateNodeVelocity(XRNode node)
    {
        Vector3 velocity = Vector3.zero;

        if (nodeManager.TryGetNodeVelocity(node, out Vector3 nodeVelocity))
        {
            // Return the magnitude of the nodeVelocity vector in its direction
            velocity = nodeVelocity;// * nodeVelocity.magnitude;// headTransform.forward * nodeVelocity.magnitude;//nodeForward * nodeVelocity.magnitude;
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
