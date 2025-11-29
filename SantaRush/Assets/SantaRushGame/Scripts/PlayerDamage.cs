using UnityEngine;
using UnityEngine.UI;   // Slider
using TMPro;            // TMP 텍스트
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerDamage : MonoBehaviour
{
    [Header("UI")]
    public Slider healthBar;                // 체력바
    public TextMeshProUGUI healthText;      // 체력 숫자 TMP

    [Header("체력 설정")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("무적 및 피격 설정")]
    [SerializeField] private float invincibilityTime = 1.5f;
    [SerializeField] private float knockbackForce = 5.0f;
    [SerializeField] private float stunTime = 0.2f;

    private bool isInvincible = false;

    // 컴포넌트
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private PlayerController playerController;


    // 추가: 사운드 컴포넌트
    private PlayerSound playerSound;

    [Header("낙사 설정")]
    public float deathY = -10f;

    // 추가: 이미 죽었는지 체크 (여러 번 Die() 호출 방지)
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerController = GetComponent<PlayerController>();
        playerSound = GetComponent<PlayerSound>();   // PlayerSound 가져오기

        // 체력 UI 초기화
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        if (healthText != null)
        {
            healthText.text = currentHealth + " / " + maxHealth;
        }
    }

    // 데미지 처리
    public void TakeDamage(int damage, Vector2 damageSourcePosition)
    {
        if (isInvincible || currentHealth <= 0) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // HP UI 갱신
        if (healthBar != null)
            healthBar.value = currentHealth;

        if (healthText != null)
            healthText.text = currentHealth + " / " + maxHealth;

        StartCoroutine(KnockbackRoutine(damageSourcePosition));

        if (currentHealth <= 0)
            Die();
        else
            StartCoroutine(InvincibilityRoutine());
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

    // 플레이어 깜빡임
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

    void Die()
    {
        if (isDead) return;              // 이미 죽음 처리했으면 다시 실행 안 함
        isDead = true;

        rb.linearVelocity = Vector2.zero;

        // 🔊 죽음 효과음 재생
        if (playerSound != null)
            playerSound.PlayDeath();

        var uiManager = FindFirstObjectByType<UIGameManager>();
        if (uiManager != null)
        {
            uiManager.ShowGameOver();
        }
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

            if (playerSound != null)
            playerSound.ObstaclesDump();
        }
    }

    void Update()
    {
        // 낙사 처리
        if (!isDead && transform.position.y < deathY && rb.linearVelocity.y < -8f)
            Die();
    }
}
