using UnityEngine;
using UnityEngine.XR;


public class ControllerGravityManager : MonoBehaviour
{
    public enum Direction
    {
        Neutral,
        Up,
        Down
    }

    public enum InputType
    {
        XRController,
        HandTracking
    }

    [Header("Input Settings")]
    public InputType currentInputType = InputType.XRController;
    public XRNode controllerNode = XRNode.RightHand;
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor handRayInteractor; // Reference to hand ray interactor for hand tracking

    [Header("Angle Settings")]
    public Direction direction = Direction.Neutral;
    public float margin = 10f;
    public float upAngle = 30f;
    public float downAngle = 150f;

    [Header("References")]
    public GravityToPlayer gravityScript;

    private InputDevice targetDevice;
    private Transform inputTransform;

    void Start()
    {
        // Get the VR controller device
        if (currentInputType == InputType.XRController)
        {
            targetDevice = InputDevices.GetDeviceAtXRNode(controllerNode);
        }

        // If gravityScript is not assigned, try to find it
        if (gravityScript == null)
        {
            gravityScript = FindObjectOfType<GravityToPlayer>();
            if (gravityScript == null)
            {
                Debug.LogWarning("No GravityToPlayer script found in the scene!");
            }
        }
    }

    void Update()
    {
        Vector3 upDirection = Vector3.up;
        bool inputValid = false;

        if (currentInputType == InputType.XRController)
        {
            // Update controller reference if needed
            if (!targetDevice.isValid)
            {
                targetDevice = InputDevices.GetDeviceAtXRNode(controllerNode);
                return;
            }

            // Get controller rotation
            if (targetDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rotation))
            {
                upDirection = rotation * Vector3.up;
                inputValid = true;
            }
        }
        else // Hand Tracking
        {
            if (handRayInteractor != null)
            {
                upDirection = handRayInteractor.transform.up;
                inputValid = true;
            }
        }

        if (inputValid)
        {
            // Calculate angle with world up
            float angle = Vector3.Angle(upDirection, Vector3.up);

            // Update direction based on angle
            Direction previousDirection = direction;

            if (angle < upAngle - margin)
            {
                direction = Direction.Up;
            }
            else if (angle > downAngle + margin)
            {
                direction = Direction.Down;
            }
            else if (angle > upAngle + margin && angle < downAngle - margin)
            {
                direction = Direction.Neutral;
            }

            // Update GravityToPlayer script state
            if (gravityScript != null)
            {
                if (direction == Direction.Up)
                {
                    gravityScript.enabled = true;
                }
                else if (direction == Direction.Down)
                {
                    gravityScript.enabled = false;
                }
            }
        }
    }
}