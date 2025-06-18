using UnityEngine;

public class MetroidvaniaCamera : MonoBehaviour
{
    [Header("Camera Follow Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private float smoothTime = 0.125f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float lookAheadDistance = 2f;
    [SerializeField] private float lookAheadSpeed = 0.1f;

    [Header("Camera Bounds")]
    [SerializeField] private bool useBounds = true;
    [SerializeField] private Vector2 minBounds;
    [SerializeField] private Vector2 maxBounds;

    private Camera cam;
    private Vector3 lookAheadOffset;
    private Vector3 targetPosition;

    void Start()
    {
        cam = GetComponent<Camera>();

        if (player == null)
        {
            enabled = false;
        }

        targetPosition = player.position + offset;
        transform.position = targetPosition;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        targetPosition = player.position + offset + lookAheadOffset;

        Vector3 desiredPosition = Vector3.Lerp(transform.position, targetPosition, smoothTime * Time.fixedDeltaTime * 60f);

        desiredPosition.z = transform.position.z;

        transform.position = desiredPosition;

        if (useBounds)
        {
            float camHeight = cam.orthographicSize;
            float camWidth = camHeight * cam.aspect;

            Vector3 clampedPosition = transform.position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, minBounds.x + camWidth, maxBounds.x - camWidth);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, minBounds.y + camHeight, maxBounds.y - camHeight);
            transform.position = clampedPosition;
        }

        UpdateLookAhead();
    }

    private void UpdateLookAhead()
    {
        Vector3 playerVelocity = player.GetComponent<Rigidbody2D>().linearVelocity;
        Vector3 targetLookAhead = Vector3.zero;

        if (playerVelocity.magnitude > 0.1f)
        {
            targetLookAhead = new Vector3(playerVelocity.x, playerVelocity.y, 0).normalized * lookAheadDistance;
        }

        lookAheadOffset = Vector3.Lerp(lookAheadOffset, targetLookAhead, lookAheadSpeed * Time.fixedDeltaTime);
    }


    public void SetCameraBounds(Vector2 newMinBounds, Vector2 newMaxBounds)
    {
        minBounds = newMinBounds;
        maxBounds = newMaxBounds;
    }

    public void SnapToPlayer()
    {
        if (player != null)
        {
            transform.position = new Vector3(player.position.x, player.position.y, transform.position.z) + offset;
            targetPosition = transform.position;
        }
    }
}