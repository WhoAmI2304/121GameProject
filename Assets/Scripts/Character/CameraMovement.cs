using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform player; // Reference to the player
    [SerializeField] private float smoothSpeed = 0.125f; // Smoothing speed for camera movement
    [SerializeField] private Vector3 offset; // Default offset for the camera position

    [SerializeField, Header("Offset Zones")] private float offsetZoneDistance = 10f;
    private Camera mainCamera;
    private Vector3 currentOffset;


    void Start()
    {
        mainCamera = Camera.main;
        currentOffset = offset; // Initialize currentOffset with the default offset
    }

    void Update()
    {
        if (player == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        bool followZoneHit = false;
        bool offsetZoneHit = false;

        foreach (RaycastHit hit in hits)
        {
            if (Input.GetMouseButton(1))
                offsetZoneHit = true;
            else followZoneHit = true;
        }

        if (followZoneHit)
            currentOffset = offset;
        else if (offsetZoneHit)
            currentOffset = CalculateOffset(ray.GetPoint(hits[0].distance), offsetZoneDistance);
        FollowPlayer();
    }

    void FollowPlayer()
    {
        Vector3 desiredPosition = player.position + currentOffset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }

    Vector3 CalculateOffset(Vector3 cursorPosition, float offsetMultiplier)
    {
        Vector3 direction = (cursorPosition - player.position).normalized;
        return direction * offsetMultiplier + offset;
    }
}