using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
public class DumbEnemyScript : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private bool isMovingLeft;
    [SerializeField]
    private LayerMask consideredMasks;
    [SerializeField]
    private float reachRange;
    [SerializeField]
    private int damage;
    [SerializeField]
    private Transform target;
    private CapsuleCollider2D enemyCollider;
    private int multiplier;
    private Animator animator;
    private float baseSpeed;
    //private int noGroundForFrames;
    //private float fallSpeed;
    private void Awake()
    {
        if (target == null)
        {
            target = FindFirstObjectByType<Player>()?.transform;
        }
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        //noGroundForFrames = 0;
        baseSpeed = speed;
    }

    private void Start()
    {
        isMovingLeft = Random.Range(0, 2) == 0;
        enemyCollider = GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {
        multiplier = isMovingLeft ? -1 : 1;
        //if (noGroundForFrames < 5)
        //{
        if (ShouldRotate())
        {
            isMovingLeft = !isMovingLeft;
        }
        transform.rotation = Quaternion.Euler(0, (isMovingLeft ? 0 : 180), 0);
        float currentSpeed = (!ShouldStop()) ? (speed / 60f) : 0f;
        animator.SetFloat("Speed", currentSpeed);
        if (!ShouldStop())
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
        }
        else
        {
            Attack();
        }
        //}
        //else
        //{
        //    transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);
        //    fallSpeed += 10f * Time.deltaTime;
        //}
    }

    private bool ShouldRotate()
    {
        //bool isObstacleAhead = Physics2D.CapsuleCast(enemyCollider.bounds.center, enemyCollider.size, enemyCollider.direction, 0f, new Vector2(multiplier, 0), reachRange, consideredMasks);
        //bool isGroundAhead = Physics2D.CapsuleCast(enemyCollider.bounds.center + Vector3.right * multiplier, enemyCollider.size, enemyCollider.direction, 0f, Vector2.down, reachRange, consideredMasks);
        Vector2 origin = (Vector2)enemyCollider.bounds.center + Vector2.right * multiplier * (enemyCollider.bounds.extents.x + 0.1f);
        RaycastHit2D hitObstacle = Physics2D.Raycast(origin, Vector2.right * multiplier, reachRange, consideredMasks);
        RaycastHit2D hitGround = Physics2D.Raycast(origin + Vector2.down * 0.1f, Vector2.down, 0.5f, consideredMasks);
        Debug.DrawRay(origin, Vector2.right * multiplier * reachRange, Color.red);
        Debug.DrawRay(origin + Vector2.down * 0.1f, Vector2.down * 0.5f, Color.green);
        bool isObstacleAhead = hitObstacle.collider != null;
        bool isGroundAhead = hitGround.collider != null;
        if (isObstacleAhead && hitObstacle.transform == target && isGroundAhead)
        {
            return false;
        }
        //if (!isGroundAhead)
        //{
        //    noGroundForFrames++;
        //}
        //else
        //{
        //    noGroundForFrames = 0;
        //    fallSpeed = 0;
        //}
        return !isGroundAhead || isObstacleAhead;
    }
    //Stop, not rotate!
    private bool ShouldStop()
    {
        //RaycastHit2D hit = Physics2D.CapsuleCast(enemyCollider.bounds.center, enemyCollider.size, enemyCollider.direction, 0f, new Vector2(multiplier, 0), reachRange, consideredMasks);
        Vector2 origin = (Vector2)enemyCollider.bounds.center + Vector2.right * multiplier * (enemyCollider.bounds.extents.x + 0.1f);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * multiplier, reachRange, consideredMasks);
        if (hit && hit.transform == target)
        {
            return true;
        }
        return false;
    }

    private void Attack()
    {
        target.gameObject.GetComponent<Player>().TakeDamage(damage);
        //Attack animation Here
    }

    public void SpeedChange(float change)
    {
        speed = baseSpeed + baseSpeed * change;
    }
}
