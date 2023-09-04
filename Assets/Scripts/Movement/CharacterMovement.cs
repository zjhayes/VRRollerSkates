using UnityEngine;
using UnityEngine.XR;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField]
    private CharacterController controller;
    [SerializeField]
    private XRNodeManager nodeManager;
    [SerializeField]
    private float boostMultiplier = 1f;
    [SerializeField]
    private float boostThreshold = 1f;
    [SerializeField]
    private float maximumVelocity = 10f;
    private Vector3 momentum;
    public float gravity = 9.81f;
    public float slopeFriction = 0.5f;
    private float momentumDecay = 0.9f;

    private void Start()
    {
        momentum = Vector3.zero;
    }

    /*private void Update()
    {
        Vector3 velocity = Vector3.zero; // controller.velocity;

        if (TryGetNodeVelocity(XRNode.LeftHand, out Vector3 leftHandVelocity))
        {
            if (leftHandVelocity.magnitude > boostThreshold)
            {
                velocity += transform.forward * leftHandVelocity.magnitude; //transform.TransformDirection(leftHandVelocity);
            }
        }

        if(TryGetNodeVelocity(XRNode.RightHand, out Vector3 rightHandVelocity))
        {
            if (rightHandVelocity.magnitude > boostThreshold)
            {
                velocity += transform.forward * rightHandVelocity.magnitude;
            }
        }

        if (TryGetNodeVelocity(XRNode.Head, out Vector3 headVelocity))
        {
            //Debug.Log("Head: " + headVelocity.magnitude);
            //velocity /= headVelocity.magnitude;
        }

        Vector3 moveDirection = velocity * boostMultiplier;
        controller.Move(moveDirection * Time.deltaTime);
    }*/

    private void Update()
    {
        // Calculate forward motion
        Vector3 currentVelocity = controller.velocity;
        /*
        if(controller.velocity != Vector3.zero)
        {
            Debug.Log("XR Moving");
        }

        if (controller.velocity.sqrMagnitude > momentum.sqrMagnitude)
        {
            momentum = new Vector3(controller.velocity.x, 0f, controller.velocity.z);
            if (momentum.magnitude > maximumVelocity)
            {
                momentum = momentum.normalized * maximumVelocity;
            }
        }*/

        Vector3 moveDirection = momentum;

        //momentum *= 0.9f; // friction

        // Apply gravity
        moveDirection.y -= gravity * Time.deltaTime;

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
            Debug.Log(slopeAngle);

            // Calculate a speed multiplier based on the slope angle
            float slopeSpeedMultiplier = Mathf.Lerp(1f, 5f, slopeAngle / 45f);

            // Apply the speed multiplier to the move direction
            moveDirection = Vector3.ProjectOnPlane(moveDirection, groundNormal) * slopeSpeedMultiplier;
        }
        else
        {
            momentum *= momentumDecay;
        }

        momentum += currentVelocity;

        if(momentum.magnitude > maximumVelocity)
        {
            momentum = momentum.normalized * maximumVelocity;
        }    

        //moveDirection *= slopeFriction; // Apply slope friction.

        /*
        // Adjust movement for slopes
        //if (slopeAngle > controller.slopeLimit)
        if (slopeAngle > 0f)
        {
            // Calculate a speed multiplier based on the slope angle
            float slopeSpeedMultiplier = Mathf.Lerp(1f, 5f, slopeAngle / 45f);

            // Apply the speed multiplier to the move direction
            moveDirection = Vector3.ProjectOnPlane(moveDirection, groundNormal) * slopeSpeedMultiplier;

            //moveDirection *= slopeFriction; // Apply slope friction.
        }*/


        // Move the character
        controller.Move(moveDirection * Time.deltaTime);
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
