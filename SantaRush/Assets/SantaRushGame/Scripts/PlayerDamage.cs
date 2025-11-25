using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

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

    [Header("낙사 설정")]
    public float deathY = -10f;

    void Start()
    {
        currentHealth = maxHealth;

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerController = GetComponent<PlayerController>();

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
            ReloadScene();  // ★ 씬 리로드
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

    // ★ 씬 리로드 함수
    void ReloadScene()
    {
        Debug.Log("산타 사망 → 씬 다시 로드!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        // y가 deathY 아래 + 충분히 떨어지고 있는 속도일 때
        if (transform.position.y < deathY && rb.linearVelocity.y < -8f)
        {
            Debug.Log("낙사로 사망!");
            ReloadScene();   // ★ 낙사도 씬 리로드
        }
    }
}
