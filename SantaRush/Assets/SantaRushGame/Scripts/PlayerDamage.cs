using UnityEngine;
using System.Collections;

public class PlayerDamage : MonoBehaviour
{
    [Header("체력 설정")]
    [SerializeField] private int maxHealth = 100; 
    private int currentHealth;

    [Header("무적 및 피격 설정")]
    [SerializeField] private float invincibilityTime = 1.5f; 
    [SerializeField] private float knockbackForce = 5.0f;     
    [SerializeField] private float stunTime = 0.2f;           

    private bool isInvincible = false;

    // 컴포넌트 참조
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private PlayerController playerController;

    // ⭐ 시작 위치 저장용
    private Vector3 startPosition;

    [Header("낙사 설정")]
    public float deathY = -10f;  

    void Start()
    {
        currentHealth = maxHealth;

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerController = GetComponent<PlayerController>();

        // ⭐ 현재 위치를 시작 위치로 저장
        startPosition = transform.position;

        Debug.Log($"게임 시작! 산타 체력: {currentHealth}");
    }

    // 데미지 받기
    public void TakeDamage(int damage, Vector2 damageSourcePosition)
    {
        if (isInvincible || currentHealth <= 0) return;

        currentHealth -= damage;
        Debug.Log($"데미지! HP: {currentHealth}/{maxHealth}");

        StartCoroutine(KnockbackRoutine(damageSourcePosition));

        if (currentHealth <= 0)
        {
            Respawn();
        }
        else
        {
            StartCoroutine(InvincibilityRoutine());
        }
    }

    public void TakeDamage(int damage)
    {
        TakeDamage(damage, transform.position);
    }

    // 넉백
    IEnumerator KnockbackRoutine(Vector2 sourcePos)
    {
        if (playerController != null) 
            playerController.enabled = false;

        Vector2 knockbackDir = (transform.position - (Vector3)sourcePos).normalized;
        knockbackDir += Vector2.up * 0.5f;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(stunTime);

        if (currentHealth > 0 && playerController != null)
            playerController.enabled = true;
    }

    // 무적 깜빡임
    IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;

        Color hitColor = new Color(1f, 0.6f, 0.6f, 1f);

        float elapsed = 0f;
        while (elapsed < invincibilityTime)
        {
            spriteRenderer.color = hitColor;
            yield return new WaitForSeconds(0.1f);

            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f);

            elapsed += 0.2f;
        }

        spriteRenderer.color = Color.white;
        isInvincible = false;
    }

    // ⭐ 사망 → 리스폰
    void Respawn()
    {
        Debug.Log("산타 사망 → 리스폰!");

        // 체력 복구
        currentHealth = maxHealth;

        // 위치 초기화
        transform.position = startPosition;

        // 조작 다시 켜기
        if (playerController != null) 
            playerController.enabled = true;

        // 속도 초기화
        rb.linearVelocity = Vector2.zero;

        // 무적 해제
        isInvincible = false;

        // 색상 초기화
        spriteRenderer.color = Color.white;

        Debug.Log("처음 위치로 돌아가기!");
    }

    // 적 충돌
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            string enemyName = collision.gameObject.name;
            int dmg = DamageManager.Instance.GetDamageByName(enemyName);

            TakeDamage(dmg, collision.transform.position);
        }
    }

    // 장애물 충돌
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            string obstacleName = collision.gameObject.name;
            int dmg = DamageManager.Instance.GetDamageByName(obstacleName);

            TakeDamage(dmg, collision.transform.position);
        }
    }

    // 낙사 처리
    void Update()
    {
        // Rigidbody 가져오기 (필요하면 Start에서 선언해도 됨)
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        // ⭐ 위치 조건 + 속도 조건 둘 다 충족해야 낙사 처리
        if (transform.position.y < deathY && rb.linearVelocity.y < -8f)
        {
            Debug.Log("낙사로 사망!");
            Respawn();
        }
    }
}
