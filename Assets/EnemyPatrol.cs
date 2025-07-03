using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private float patrolDistance = 3f; // ��������� �������������� (����� � ������ �� ��������� �����)
    [SerializeField] private float moveSpeed = 2f; // �������� �������� �����

    private Rigidbody2D rb;
    private Vector2 startPosition;
    private bool movingRight = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position; // ���������� ��������� �������
        Flip();
    }

    private void FixedUpdate()
    {
        Patrol();
    }

    private void Patrol()
    {
        // ���������� ������� �������
        float targetX = movingRight ? startPosition.x + patrolDistance : startPosition.x - patrolDistance;

        // ������� �����
        Vector2 targetVelocity = new Vector2(movingRight ? moveSpeed : -moveSpeed, rb.linearVelocity.y);
        rb.linearVelocity = targetVelocity;

        // ���������, ������ �� ���� ������� �������
        if (movingRight && transform.position.x >= startPosition.x + patrolDistance)
        {
            movingRight = false;
            Flip();
        }
        else if (!movingRight && transform.position.x <= startPosition.x - patrolDistance)
        {
            movingRight = true;
            Flip();
        }
    }

    private void Flip()
    {
        // �������������� ������ ����� �� ��� X
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}