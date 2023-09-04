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
