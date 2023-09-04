using UnityEngine;

public class XRManager : MonoBehaviour
{
    [SerializeField]
    private XRNodeManager nodeManager;

    public XRNodeManager NodeManager
    {
        get { return nodeManager; }
    }
}
