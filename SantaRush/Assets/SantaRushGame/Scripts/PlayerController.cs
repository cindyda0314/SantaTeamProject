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

    // ⭐ 점프 입력 버퍼(절대 씹히지 않음)
    private bool jumpQueued = false;

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

        // ⭐ 점프 입력 저장(버퍼)
        if (Input.GetKeyDown(KeyCode.Space))
            jumpQueued = true;

        // Ground 체크
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        if (isGrounded)
        {
            jumpCount = 0;
            animator.SetBool("isJumping", false);
        }

        // 애니메이션 이동값
        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
    }

    void FixedUpdate()
    {
        // ⭐ FixedUpdate에서 점프 처리 → 입력 안 씹힘
        if (jumpQueued)
        {
            // 1단 점프
            if (isGrounded && jumpCount == 0)
            {
                Jump();
            }
            // 2단 점프
            else if (!isGrounded && jumpCount == 1 && enableDoubleJump)
            {
                Jump();
            }

            // 입력 소모
            jumpQueued = false;
        }
    }

    void Jump()
    {
        jumpCount++;

        float force = jumpForce;

        // 2단 점프는 살짝 높게(너 기존 코드 유지)
        if (jumpCount == 2)
            force = jumpForce * 1.3f;

        // 점프 힘 적용
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, force);

        animator.SetBool("isJumping", true);
    }

    // Jump 애니메이션 끝 이벤트
    public void OnJumpAnimationEnd()
    {
        if (animator != null)
            animator.SetBool("isJumping", false);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }
}
