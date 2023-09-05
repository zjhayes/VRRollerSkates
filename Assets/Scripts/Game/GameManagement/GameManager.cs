using UnityEngine;

public class GameManager : MonoBehaviour, IGameManager
{
    [SerializeField]
    private XRManager xr;
    [SerializeField]
    private PlayerManager player;

    private void Awake()
    {
        // Inject gameManager into dependents.
        ServiceInjector.Resolve<IGameManager, GameBehaviour>(this);
    }

    public XRManager XR
    { 
        get { return xr; }
    }

    public PlayerManager Player
    {
        get { return player; }
    }
}
