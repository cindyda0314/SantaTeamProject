using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 5.0f;

    [Header("점프 설정")]
    public float jumpForce = 10.0f;
    public bool enableDoubleJump = true;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [Header("Ground 체크")]
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;

    private bool isGrounded = false;
    private int jumpCount = 0;

    // 빠른 더블점프 입력
    private float lastJumpTime = 0f;
    public float doubleJumpInputDelay = 0.25f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // 이동 입력
        float moveX = 0f;
        if (Input.GetKey(KeyCode.A)) moveX = -1f;
        if (Input.GetKey(KeyCode.D)) moveX = 1f;

        rb.linearVelocity = new Vector2(moveX * moveSpeed, rb.linearVelocity.y);

        // 좌우 반전
        if (moveX < 0) spriteRenderer.flipX = true;
        if (moveX > 0) spriteRenderer.flipX = false;

        // Ground 체크
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        if (isGrounded)
        {
            jumpCount = 0;
            animator.SetBool("isJumping", false);  // 착지 → 점프 종료
        }

        // 점프 / 더블점프
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 1단 점프
            if (isGrounded && jumpCount == 0)
            {
                Jump();
                lastJumpTime = Time.time;
            }
            // 2단 점프 (빠르게 눌러야)
            else if (!isGrounded && jumpCount == 1 && enableDoubleJump)
            {
                if (Time.time - lastJumpTime <= doubleJumpInputDelay)
                    Jump();
            }
        }

        // 이동 속도 애니메이션
        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
    }

    void Jump()
    {
        jumpCount++;

        float force = jumpForce;

        // 더블점프는 더 높게
        if (jumpCount == 2)
            force = jumpForce * 1.3f;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, force);

        animator.SetBool("isJumping", true);  // 점프 애니메이션 시작
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }
}

