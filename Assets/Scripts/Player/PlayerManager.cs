using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private CharacterController controller;
    [SerializeField]
    private PlayerMovement movement;
    [SerializeField]
    private PlayerPhysics physics;

    public CharacterController Controller
    {
        get { return controller; }
    }

    public PlayerMovement Movement
    {
        get { return movement; }
    }

    public PlayerPhysics Physics
    {
        get { return physics; }
    }
}
