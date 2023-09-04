using UnityEngine;

public class GameManager : MonoBehaviour, IGameManager
{
    [SerializeField]
    private XRManager xr;

    private void Awake()
    {
        // Inject gameManager into dependents.
        ServiceInjector.Resolve<IGameManager, GameBehaviour>(this);
    }

    public XRManager XR
    { 
        get { return xr; }
    }
}
