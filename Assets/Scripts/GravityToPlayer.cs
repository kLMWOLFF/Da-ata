using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class GravityToPlayer : MonoBehaviour

{
    public Transform playerCenter;             // The camera or XR Origin
    public float gravityStrength = 5f;         // How strong the attraction is
    public float floatForceStrength = 0.2f;    // How much it floats around
    public float gazeAttractDistance = 10f;    // Max distance for gaze interaction
    public float gazeAngleThreshold = 15f;     // Field of view angle to trigger pull

    private Rigidbody rb;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    }

    void FixedUpdate()
    {
        if (grabInteractable != null && grabInteractable.isSelected)
            return;

        ApplyFloating();

        if (IsGazedAt())
        {
            AttractToPlayer();
        }
    }

    void ApplyFloating()
    {
        Vector3 randomFloat = new Vector3(
            Mathf.PerlinNoise(Time.time * 0.5f, 0f) - 0.5f,
            Mathf.PerlinNoise(0f, Time.time * 0.5f) - 0.5f,
            Mathf.PerlinNoise(Time.time * 0.3f, Time.time * 0.3f) - 0.5f
        ) * floatForceStrength;

        rb.AddForce(randomFloat, ForceMode.Acceleration);
    }

    void AttractToPlayer()
    {
        Vector3 direction = (playerCenter.position - transform.position).normalized;
        float distance = Vector3.Distance(playerCenter.position, transform.position);
        float forceMagnitude = gravityStrength / Mathf.Max(distance, 0.5f);
        rb.AddForce(direction * forceMagnitude, ForceMode.Acceleration);
    }

    bool IsGazedAt()
    {
        Vector3 cameraForward = playerCenter.forward;
        Vector3 toObject = (transform.position - playerCenter.position).normalized;
        float angle = Vector3.Angle(cameraForward, toObject);

        if (angle < gazeAngleThreshold)
        {
            Ray gazeRay = new Ray(playerCenter.position, cameraForward);
            if (Physics.Raycast(gazeRay, out RaycastHit hit, gazeAttractDistance))
            {
                return hit.transform == transform;
            }
        }

        return false;
    }
    public bool IsBeingControlled()
    {
        return IsGazedAt();
    }

}
