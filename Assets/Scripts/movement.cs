using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 50f;
    [SerializeField] private float airControl = 0.8f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private int maxJumps = 2;
    [SerializeField] private float jumpCutMultiplier = 0.5f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.1f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float dashCooldown = 0.5f;
    [SerializeField] private bool canDashInAir = true;

    [Header("Ground")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.4f, 0.1f);
    [SerializeField] private Transform groundCheck;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private float moveInput;
    private bool isFacingRight = false;
    private bool isGrounded;
    private int jumpsLeft;
    private float coyoteTimer;
    private float jumpBufferTimer;
    private bool isDashing;
    private float dashTimer;
    private float dashCooldownTimer;
    private Vector2 lastVelocity;

    private float baseGravity;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        jumpsLeft = maxJumps;
        baseGravity = rb.gravityScale;
    }

    private void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        // ������� �������� �� ��������������� ������������� ����� (��� OnTriggerEnter2D)
        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);

        // ����� ���������� ���� ���� ��� �� �� �����, � ���� ������ ������
        if (isGrounded)
        {
            coyoteTimer = coyoteTime;
            jumpsLeft = maxJumps;
        }
        else if (!isGrounded && jumpsLeft == maxJumps)
        {
            jumpsLeft -= 1;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        // ����� ���� ������ ����� � ������� ���� ������ ��������� �� �����, �������� ��� ����� �������, �������������, � ����� �� �������
        // ������ Jump ������������� � ���������� �������
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferTimer = jumpBufferTime;
        }
        else
        {
            jumpBufferTimer -= Time.deltaTime;
        }
        
        // �������� ������� ������, ������� ������ � �������� �� ��� ������
        if (jumpBufferTimer > 0f && (coyoteTimer > 0f || jumpsLeft > 0))
        {
            Jump();
        }

        // ������ ������, ���� �� ��������� ������ ������
        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0f && (!isDashing && (canDashInAir || isGrounded)))
        {
            StartDash();
        }

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
                isDashing = false;
        }

        dashCooldownTimer -= Time.deltaTime;
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        // �� ���� ��������� ��� ����
        if (!isDashing)
        {
            Move();
        }
    }

    private void Move()
    {
        float targetSpeed = moveInput * moveSpeed;

        // ������������� �������� �� ������������� ��������
        float speedDif = targetSpeed - rb.linearVelocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;

        // � ������� ��������� �������� ���������
        accelRate *= (isGrounded ? 1f : airControl);
        float movement = speedDif * accelRate;
        rb.AddForce(movement * Vector2.right);

        // ���� �� ��������, ��� ��� �������� ���������� ������ �������, � �� ������, �� ��� ����
        if (moveInput != 0)
        {
            isFacingRight = moveInput > 0;
            spriteRenderer.flipX = !isFacingRight;
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        jumpsLeft--;
        coyoteTimer = 0f;
        jumpBufferTimer = 0f;
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimer = dashTime;
        dashCooldownTimer = dashCooldown;
        lastVelocity = rb.linearVelocity;
        rb.linearVelocity = new Vector2((isFacingRight ? 1 : -1) * dashSpeed, 0f);
        rb.gravityScale = 0f;
    }

    private void UpdateAnimations()
    {
        animator.SetFloat("Speed", Mathf.Abs(moveInput));
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsJumping", !isGrounded && rb.linearVelocity.y > 0f);
        animator.SetBool("IsFalling", !isGrounded && rb.linearVelocity.y < 0f);
}

    private void LateUpdate()
    {
        // ���� � ���� ���������� �� ���������, ����� ������������� �������� ���������
        if (!isDashing)
        {
            rb.gravityScale = baseGravity;
        }
    }

    // ��� �������� �������������� ��������� ����� IsGroundedIsGrounded
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        }
    }
}