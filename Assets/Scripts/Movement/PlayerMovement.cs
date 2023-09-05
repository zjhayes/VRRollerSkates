using UnityEngine;


public class PlayerMovement : GameBehaviour
{
    [SerializeField]
    GestureMoveProvider moveProvider;

    private void Start()
    {
        moveProvider.HandleMovementFromHandMotion();

        gameManager.Player.Physics.OnUpdateMomentum += moveProvider.HandleMovementFromHandMotion;
    }
}