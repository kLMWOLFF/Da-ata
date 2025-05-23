using UnityEngine;
using System.Collections.Generic;

public class GravityToPlayer : MonoBehaviour
{
    public Transform playerCenter;             // The camera or XR Origin
    public float gravityStrength = 5f;         // How strong the attraction is
    public float floatForceStrength = 0.2f;    // How much it floats around
    public float massInfluence = 2f;           // How much mass affects the gravity force
    
    private ControllerGravityManager gravityManager;
    private List<Rigidbody> affectedObjects = new List<Rigidbody>();

    void Start()
    {
        // Find the ControllerGravityManager in the scene
        gravityManager = FindObjectOfType<ControllerGravityManager>();
        if (gravityManager == null)
        {
            Debug.LogWarning("No ControllerGravityManager found in the scene!");
        }

        // Find all objects with GravityObjects tag
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag("GravityObjects");
        foreach (GameObject obj in taggedObjects)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                affectedObjects.Add(rb);
            }
            else
            {
                Debug.LogWarning($"Object {obj.name} has GravityObjects tag but no Rigidbody component!");
            }
        }
    }

    void FixedUpdate()
    {
        foreach (Rigidbody rb in affectedObjects.ToArray()) // Use ToArray to avoid collection modification issues
        {
            if (rb == null) continue; // Skip if object was destroyed

            // Check if object is being grabbed
            var grabInteractable = rb.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            if (grabInteractable != null && grabInteractable.isSelected) continue;

            ApplyFloating(rb);

            // Check the gravity manager's direction and apply forces accordingly
            if (gravityManager != null)
            {
                if (gravityManager.direction == ControllerGravityManager.Direction.Up)
                {
                    AttractToPlayer(rb);
                }
                else if (gravityManager.direction == ControllerGravityManager.Direction.Down)
                {
                    RepelFromPlayer(rb);
                }
            }
        }
    }

    void ApplyFloating(Rigidbody rb)
    {
        Vector3 randomFloat = new Vector3(
            Mathf.PerlinNoise(Time.time * 0.5f + rb.position.x, 0f) - 0.5f,
            Mathf.PerlinNoise(0f, Time.time * 0.5f + rb.position.y) - 0.5f,
            Mathf.PerlinNoise(Time.time * 0.3f, Time.time * 0.3f + rb.position.z) - 0.5f
        ) * floatForceStrength;

        rb.AddForce(randomFloat, ForceMode.Acceleration);
    }

    void AttractToPlayer(Rigidbody rb)
    {
        Vector3 direction = (playerCenter.position - rb.position).normalized;
        float distance = Vector3.Distance(playerCenter.position, rb.position);
        
        // Calculate force magnitude based on distance and mass
        float baseForceMagnitude = gravityStrength / Mathf.Max(distance, 0.5f);
        float massMultiplier = Mathf.Pow(rb.mass, massInfluence);
        float forceMagnitude = baseForceMagnitude * massMultiplier;
        
        rb.AddForce(direction * forceMagnitude, ForceMode.Acceleration);
    }

    void RepelFromPlayer(Rigidbody rb)
    {
        Vector3 direction = (rb.position - playerCenter.position).normalized;
        float distance = Vector3.Distance(playerCenter.position, rb.position);
        
        // Calculate force magnitude based on distance and mass
        float baseForceMagnitude = gravityStrength / Mathf.Max(distance, 0.5f);
        float massMultiplier = Mathf.Pow(rb.mass, massInfluence);
        float forceMagnitude = baseForceMagnitude * massMultiplier;
        
        rb.AddForce(direction * forceMagnitude, ForceMode.Acceleration);
    }

    // Called when a new object with GravityObjects tag is added to the scene
    void OnLevelWasLoaded()
    {
        Start(); // Refresh the list of affected objects
    }
}