using UnityEngine;
using System.Collections;

public class PlayerDamage : MonoBehaviour
{
    [Header("체력 설정")]
    [SerializeField] private int maxHealth = 100; // 최대 체력 100
    private int currentHealth;

    [Header("무적 및 피격 설정")]
    [SerializeField] private float invincibilityTime = 1.5f; // 무적 지속 시간
    [SerializeField] private float knockbackForce = 5.0f;    // 튕겨나가는 힘
    [SerializeField] private float stunTime = 0.2f;          // 경직 시간

    private bool isInvincible = false;

    // 컴포넌트 참조
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private PlayerController playerController; 

    void Start()
    {
        currentHealth = maxHealth;
        
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerController = GetComponent<PlayerController>();
        
        // 게임 시작 시 체력 확인용 로그 하나만 남김 (필요 없으면 지워도 됨)
        Debug.Log($"게임 시작! 산타 체력: {currentHealth}");
    }

    // 외부에서 데미지를 줄 때 (위치 정보 포함)
    public void TakeDamage(int damage, Vector2 damageSourcePosition)
    {
        if (isInvincible || currentHealth <= 0) return;

        currentHealth -= damage;

        // 현재 체력 상태 출력 (디버깅용이 아니라 게임 플레이 정보 확인용으로 남김)
        Debug.Log($"데미지 입음! 현재 HP: {currentHealth}/{maxHealth}");

        StartCoroutine(KnockbackRoutine(damageSourcePosition));

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityRoutine());
        }
    }

    // 위치 정보 없이 데미지만 줄 때
    public void TakeDamage(int damage)
    {
        TakeDamage(damage, transform.position);
    }

    // 넉백 코루틴
    IEnumerator KnockbackRoutine(Vector2 sourcePos)
    {
        if (playerController != null) playerController.enabled = false;

        Vector2 knockbackDir = (transform.position - (Vector3)sourcePos).normalized;
        knockbackDir += Vector2.up * 0.5f;
        
        rb.linearVelocity = Vector2.zero; 
        rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(stunTime);

        if (currentHealth > 0 && playerController != null) 
            playerController.enabled = true;
    }

    // 무적 효과 코루틴 (살짝 붉은색)
    IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;

        // 살짝 붉은색 (R:1, G:0.6, B:0.6)
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
        Debug.Log("산타 사망!");
        animator.SetTrigger("Die");

        // 1. 플레이어 조작 끄기
        if (playerController != null) playerController.enabled = false;
        
        // 2. ★ 여기가 수정됨! ★
        // 충돌체를 끄는 코드를 지웠습니다. 이제 바닥 위에 시체가 남습니다.
        // GetComponent<Collider2D>().enabled = false;  <-- 이 줄 삭제함
        
        // 3. 움직임 멈추기
        rb.linearVelocity = Vector2.zero;

        // (선택 사항) 죽은 뒤에 누가 밀어도 안 밀리게 하려면 아래 주석을 해제하세요.
        // rb.bodyType = RigidbodyType2D.Static; 

        this.enabled = false; 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // 적의 이름 가져오기
            string enemyName = collision.gameObject.name;
            
            // 매니저에게 데미지 물어보기 (로그 출력 없이 바로 계산)
            int dmg = EnemyDamageManager.Instance.GetDamageByName(enemyName);
            
            // 데미지 적용
            TakeDamage(dmg, collision.transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Trap"))
        {
            // 함정은 기본 10 데미지 (필요시 수정)
            TakeDamage(10, collision.transform.position);
        }
    }
}