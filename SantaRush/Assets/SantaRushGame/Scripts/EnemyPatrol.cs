using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Waypoint Settings")]
    [SerializeField] private Transform waypointA;    // 첫 번째 순찰 지점
    [SerializeField] private Transform waypointB;    // 두 번째 순찰 지점
    
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;   // 이동 속도
    
    private Transform currentTarget;                 // 현재 목표 지점
    private SpriteRenderer spriteRenderer;           // 스프라이트 좌우 반전용
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentTarget = waypointB;  // 시작 시 B를 향해 이동
    }
    
    void Update()
    {
        Patrol();
    }
    
    void Patrol()
    {
        // 목표 지점을 향해 이동
        transform.position = Vector2.MoveTowards(
            transform.position,
            currentTarget.position,
            moveSpeed * Time.deltaTime
        );
        
        // 목표 지점에 도달했는지 확인
        float distance = Vector2.Distance(transform.position, currentTarget.position);
        
        if (distance < 0.1f)
        {
            // 목표 변경 (A <-> B)
            if (currentTarget == waypointA)
            {
                currentTarget = waypointB;
                spriteRenderer.flipX = true;  // 오른쪽 보기
            }
            else
            {
                currentTarget = waypointA;
                spriteRenderer.flipX = false;   // 왼쪽 보기
            }
        }
    }
}

