using UnityEngine;

public class BookFollow : MonoBehaviour
{
    // Скрипт для летающей книги для демона-мага, можно поменять на посох
    [SerializeField] private Transform targetPoint;
    [SerializeField] private float smoothTime = 0.125f;
    [SerializeField] private float maxDistance = 2f;
    [SerializeField] private float minDistance = 0.1f;
    [SerializeField] private Vector2 offset;

    private Vector2 currentPosition;
    private Vector2 targetPosition;

    private void Start()
    {
        if (targetPoint == null)
        {
            enabled = false;
            return;
        }

        currentPosition = transform.position;
        targetPosition = (Vector2)targetPoint.position + offset;
        transform.position = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);
    }

    private void FixedUpdate()
    {
        if (targetPoint == null) return;

        targetPosition = (Vector2)targetPoint.position + offset;
        currentPosition = transform.position;
        float distance = Vector2.Distance(currentPosition, targetPosition);

        if (distance > minDistance)
        {
            Vector2 newPosition = Vector2.Lerp(
                currentPosition,
                targetPosition,
                smoothTime * Time.fixedDeltaTime * 60f
            );

            if (distance > maxDistance)
            {
                newPosition = targetPosition + (newPosition - targetPosition).normalized * maxDistance;
            }

            transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
        }
    }
}