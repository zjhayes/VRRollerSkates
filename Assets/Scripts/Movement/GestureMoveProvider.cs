using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class GestureMoveProvider : GameBehaviour
{
    [SerializeField]
    private float motionMagnitudeThreshold = 1f;

    public List<Vector3> GetControllerVelocities()
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

    public Vector3 CalculateForwardDirectionFromInput(Vector3 input)
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

    public float CalculateForwardMagnitudeFromInput(Vector3 input)
    {
        return input.magnitude;
    }

    public float CalculateRotationFromInput(Vector3 input)
    {
        Vector3 direction = input.normalized;

        // Extract the X-axis motion for rotation.
        return direction.x;
    }

    private bool TryCalculateControllerVelocity(XRNode node, out Vector3 handVelocity)
    {
        if (gameManager.XR.NodeManager.TryGetNodeVelocity(node, out handVelocity) && 
            handVelocity.magnitude > motionMagnitudeThreshold)
        {
            return true;
        }
        else return false;
    }
}
