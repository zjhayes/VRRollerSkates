using UnityEngine;

public class CharacterSlide : MonoBehaviour
{
    [SerializeField]
    public float moveSpeed = 5f;
    [SerializeField]
    public float slopeForce = 10f;

    private CharacterController characterController;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // Cast a ray to detect the slope
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, 1.5f))
        {
            // Calculate the slope angle
            float slopeAngle = Vector3.Angle(hitInfo.normal, Vector3.up);

            // Calculate the downward force based on the slope
            float forceMagnitude = slopeAngle * slopeForce;

            // Apply the downward force
            characterController.Move(Vector3.down * forceMagnitude * Time.deltaTime);

            // Calculate movement along the slope
            Vector3 moveDirection = Vector3.Cross(hitInfo.normal, Vector3.down);
            moveDirection.Normalize();

            // Apply movement along the slope
            characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
        }
    }
}
