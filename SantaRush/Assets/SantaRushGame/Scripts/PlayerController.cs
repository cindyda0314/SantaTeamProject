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

    // 점프 입력 버퍼
    private bool jumpQueued = false;

    // 사운드 스크립트
    private PlayerSound playerSound;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 같은 오브젝트에 붙어 있는 PlayerSound 찾기
        playerSound = GetComponent<PlayerSound>();
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

        // 점프 입력 저장
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
        // 점프 처리
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

        // 2단 점프는 살짝 높게
        if (jumpCount == 2)
            force = jumpForce * 1.3f;

        // 점프 힘 적용
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, force);

        animator.SetBool("isJumping", true);

        // 점프 사운드
        if (playerSound != null)
            playerSound.PlayJump();
    }

    // ---- 충돌 처리 + 사운드 ----

    void OnCollisionEnter2D(Collision2D collision)
    {
        ProcessHit(collision.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        ProcessHit(other.gameObject);
    }

    void ProcessHit(GameObject other)
    {
        if (playerSound == null) return;

        // 적
        if (other.CompareTag("Enemy"))
        {
            playerSound.EnemyDump();
        }
        // 장애물
        else if (other.CompareTag("Obstacle"))
        {
            playerSound.ObstaclesDump();
        }
        // 오너먼트 아이템
        else if (other.CompareTag("Ornament"))
        {
            playerSound.ItemOrnament();
        }
        // 크리스마스 양말 아이템
        else if (other.CompareTag("Christmasstocking"))
        {
            playerSound.ItemChristmasstocking();
        }
        // 리스 아이템
        else if (other.CompareTag("Wreath"))
        {
            playerSound.ItemWreath();
        }
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
