using UnityEngine;
using UnityEngine.XR;

public class ControllerGravityManager : MonoBehaviour
{
    [Header("Controller Angles")]
    [SerializeField] private float pullMinZAngle = 0f;
    [SerializeField] private float pullMaxZAngle = 180f;
    [SerializeField] private float pushMinZAngle = 180f;
    [SerializeField] private float pushMaxZAngle = 360f;

    [Header("References")]
    [SerializeField] private GravityToPlayer gravityScript;

    private XRNode rightHandNode = XRNode.RightHand;
    private float originalGravityStrength;
    private float originalFloatForce;

    void Start()
    {
        if (gravityScript == null)
        {
            Debug.LogError("Please assign the GravityToPlayer script reference!");
            enabled = false;
            return;
        }

        // Store original values
        originalGravityStrength = gravityScript.gravityStrength;
        originalFloatForce = gravityScript.floatForceStrength;
    }

    void Update()
    {
        InputDevice rightController = InputDevices.GetDeviceAtXRNode(rightHandNode);
        rightController.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rotation);
        float zAngle = rotation.eulerAngles.z;

        if (zAngle >= pullMinZAngle && zAngle <= pullMaxZAngle)
        {
            // Enhance pull effect
            gravityScript.gravityStrength = originalGravityStrength * 2f;
        }
        else if (zAngle >= pushMinZAngle && zAngle <= pushMaxZAngle)
        {
            // Reverse gravity direction
            gravityScript.gravityStrength = -originalGravityStrength;
        }
        else
        {
            // Reset to original values
            gravityScript.gravityStrength = originalGravityStrength;
        }
    }
}