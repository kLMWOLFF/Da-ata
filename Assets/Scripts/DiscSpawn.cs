using UnityEngine;
using UnityEngine.XR;

public class DiscSpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    [SerializeField] private GameObject discPrefab;
    [SerializeField] private float spawnDistance = 2f;
    [SerializeField] private Transform playerCenter;

    [Header("Controller Settings")]
    [SerializeField] private float pushMinZAngle = 220f;
    [SerializeField] private float pushMaxZAngle = 260f;

    [Header("Gaze Settings")]
    [SerializeField] private float gazeAngleThreshold = 15f;
    [SerializeField] private float gazeAttractDistance = 20f;

    private XRNode rightHandNode = XRNode.RightHand;
    private float spawnCooldown = 0.5f;
    private float lastSpawnTime;

    void Start()
    {
        if (discPrefab == null || playerCenter == null)
        {
            Debug.LogError("Please assign all required references in the inspector!");
            enabled = false;
            return;
        }
    }

    void Update()
    {
        if (Time.time - lastSpawnTime < spawnCooldown) return;

        if (IsGazedAt() && IsControllerInPushZone())
        {
            SpawnDisc();
            lastSpawnTime = Time.time;
        }
    }

    private bool IsControllerInPushZone()
    {
        InputDevice rightController = InputDevices.GetDeviceAtXRNode(rightHandNode);
        rightController.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rotation);
        float zAngle = rotation.eulerAngles.z;

        return zAngle >= pushMinZAngle && zAngle <= pushMaxZAngle;
    }

    private bool IsGazedAt()
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

    private void SpawnDisc()
    {
        Vector3 spawnPosition = playerCenter.position + (playerCenter.forward * spawnDistance);
        GameObject newDisc = Instantiate(discPrefab, spawnPosition, Quaternion.identity);
    }
}