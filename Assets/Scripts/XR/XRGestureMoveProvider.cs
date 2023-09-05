using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class XRGestureMoveProvider : GameBehaviour
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
