using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class XRNodeManager : MonoBehaviour
{
    Dictionary<XRNode, XRNodeState> nodeCache;

    private void Awake()
    {
        nodeCache = new Dictionary<XRNode, XRNodeState>();
        InputTracking.nodeAdded += CacheNode;
        InputTracking.nodeRemoved += RemoveNode;
    }

    private void Update()
    {
        RefreshNodes();
    }

    public bool TryGetNodeState(XRNode nodeType, out XRNodeState nodeState)
    {
        nodeState = new XRNodeState();
        if(nodeCache.TryGetValue(nodeType, out nodeState))
        {
            return true;
        }
        return false;
    }

    public bool TryGetNodePosition(XRNode node, out Vector3 position)
    {
        position = Vector3.zero;
        if (TryGetNodeState(node, out XRNodeState nodeState) &&
            nodeState.TryGetPosition(out position))
        {
            return true;
        }

        // Return zero vector, node was not found.
        return false;
    }

    // Get the rotation of the GameObject associated with the XRNode.
    public bool TryGetNodeRotation(XRNode node, out Quaternion rotation)
    {
        rotation = Quaternion.identity;
        if (TryGetNodeState(node, out XRNodeState nodeState) &&
            nodeState.TryGetRotation(out rotation))
        {
            return true;
        }

        // Return identity rotation if the node state is not available.
        return false;
    }

    public bool TryGetNodeVelocity(XRNode nodeType, out Vector3 velocity)
    {
        velocity = Vector3.zero;
        if (TryGetNodeState(nodeType, out XRNodeState nodeState) &&
            nodeState.TryGetVelocity(out velocity))
        {
            // Node velocity found.
            return true;
        }
        return false; // Couldn't get velocity.
    }

    public bool TryGetNodeForward(XRNode nodeType, out Vector3 forward)
    {
        forward = Vector3.zero;
        if (TryGetNodeState(nodeType, out XRNodeState nodeState) &&
            nodeState.TryGetRotation(out Quaternion orientation))
        {
            forward = orientation * Vector3.forward;
            return true;
        }
        return false;
    }

    public void CacheNode(XRNodeState nodeState)
    {
        // Replace current of node type.
        if(nodeCache.ContainsKey(nodeState.nodeType))
        {
            nodeCache[nodeState.nodeType] = nodeState;
        }
        else
        {
            // Add new node type.
            nodeCache.Add(nodeState.nodeType, nodeState);
        }
    }

    public void RemoveNode(XRNodeState node)
    {
        nodeCache.Remove(node.nodeType);
    }

    public void RefreshNodes()
    {
        nodeCache.Clear();
        List<XRNodeState> nodes = new List<XRNodeState>();
        InputTracking.GetNodeStates(nodes);
        foreach(XRNodeState nodeState in nodes)
        {
            CacheNode(nodeState);
        }
    }
}
