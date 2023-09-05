using UnityEngine;

public class PlayerPhysics : GameBehaviour
{
    [SerializeField]
    private float gravity = 9.81f;
    [SerializeField]
    private float maximumVelocity = 50f;
    [SerializeField]
    private float maxAngularMomentum = 10f;
    [SerializeField]
    private float momentumDecayTime = 1.0f;

    private readonly float stopVelocity = 0.001f; // The velocity threshold considered stopped.
    private Vector3 momentum;
    private float angularMomentum;

    public event Delegates.UpdateAction OnUpdateMomentum;

    private void Start()
    {
        momentum = Vector3.zero;
        angularMomentum = 0f;
    }

    private void Update()
    {
        UpdateMomentum();
        ApplyMomentumToMovement();
        ApplyAngularMomentum();
    }

    private void UpdateMomentum()
    {
        OnUpdateMomentum?.Invoke();
        ApplyGravity();
        ApplySlope();
        ApplyMomentumDecay();
        ClampMomentum();
    }

    public Vector3 Momentum
    {
        get { return momentum; }
        set { momentum = value; }
    }

    public float AngularMomentum
    {
        get { return angularMomentum; }
        set { angularMomentum = value; }
    }

    private void ApplyGravity()
    {
        momentum.y -= gravity * Time.deltaTime;
    }

    private void ApplyMomentumDecay()
    {
        if (momentum.magnitude > 0f)
        {
            // Apply smoothed momentum decay.
            momentum = Vector3.Lerp(momentum, Vector3.zero, Time.deltaTime / momentumDecayTime);
        }

        // Apply angular momentum decay
        angularMomentum = Mathf.Lerp(angularMomentum, 0f, Time.deltaTime / momentumDecayTime);
    }

    private void ApplySlope()
    {
        // Cast a ray to detect ground and calculate the ground normal.
        RaycastHit hit;
        Vector3 groundNormal = Vector3.up; // Default to upright
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, gameManager.Player.Controller.height / 2 + 0.1f))
        {
            groundNormal = hit.normal;
        }

        // Calculate the slope angle
        float slopeAngle = Vector3.Angle(Vector3.up, groundNormal);

        if (slopeAngle > 0f)
        {
            // Calculate a speed multiplier based on the slope angle
            float slopeSpeedMultiplier = Mathf.Lerp(1f, 1.5f, slopeAngle / 45f);

            // Calculate the slope-adjusted momentum direction
            Vector3 slopeAdjustedMomentum = Vector3.ProjectOnPlane(momentum, groundNormal) * slopeSpeedMultiplier;

            // Blend the slope-adjusted momentum with the original momentum
            float blendFactor = 0.5f;
            momentum = Vector3.Lerp(momentum, slopeAdjustedMomentum, blendFactor); // Adjust blending factor as needed
        }
    }

    private void ClampMomentum()
    {

        if (momentum.magnitude > maximumVelocity)
        {
            // Clamp to maximum velocity.
            momentum = momentum.normalized * maximumVelocity;
        }
        else if (momentum.magnitude < stopVelocity)
        {
            momentum = Vector3.zero;
        }

        // Clamp angularMomentum.
        angularMomentum = Mathf.Clamp(angularMomentum, -maxAngularMomentum, maxAngularMomentum);
    }

    private void ApplyMomentumToMovement()
    {
        gameManager.Player.Controller.Move(momentum * Time.deltaTime);
    }

    private void ApplyAngularMomentum()
    {
        float rotationFriction = 1.0f;

        // Calculate the rotation angle based on input and time.
        float rotationAngle = angularMomentum * rotationFriction * Time.deltaTime;

        // Rotate the character around the Y-axis based on angularMomentum.
        transform.Rotate(Vector3.up, angularMomentum);
    }
}
