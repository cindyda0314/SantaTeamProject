using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;        // 최대 체력
    private int currentHealth;                         // 현재 체력
    
    [Header("Invincibility Settings")]
    [SerializeField] private float invincibilityTime = 1f;  // 무적 시간
    private bool isInvincible = false;                      // 무적 상태 여부
    
    private SpriteRenderer spriteRenderer;
    
    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    public void TakeDamage(int damage)
    {
        if (isInvincible) return;  // 무적이면 데미지 무시
        
        currentHealth -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage. HP: " + currentHealth);
        
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityCoroutine());
        }
    }
    
    private System.Collections.IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        
        // 깜빡이는 효과 (0.1초마다 토글)
        float elapsed = 0f;
        while (elapsed < invincibilityTime)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);  // 반투명
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;                  // 원래 색
            yield return new WaitForSeconds(0.1f);
            
            elapsed += 0.2f;
        }
        
        isInvincible = false;
        spriteRenderer.color = Color.white;  // 완전 복구
    }
    
    private void Die()
    {
        Debug.Log(gameObject.name + " has died!");
        Destroy(gameObject);
    }
}