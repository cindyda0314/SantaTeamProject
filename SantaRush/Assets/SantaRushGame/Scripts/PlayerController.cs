using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5.0f;

    private Animator animator;
    private SpriteRenderer spriteRenderer;   // ⭐ 방향 전환을 위한 SpriteRenderer

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();  // ⭐ SpriteRenderer 가져오기

        if (animator != null)
            Debug.Log("Animator 컴포넌트를 찾았습니다!");
        else
            Debug.LogError("Animator 컴포넌트가 없습니다!");

        if (spriteRenderer != null)
            Debug.Log("SpriteRenderer 컴포넌트를 찾았습니다!");
        else
            Debug.LogError("SpriteRenderer가 없습니다!");
    }

    void Update()
    {
        Vector3 movement = Vector3.zero;

        // 이동 방향 감지
        if (Input.GetKey(KeyCode.A))
        {
            movement += Vector3.left;
            spriteRenderer.flipX = true;     // ⭐ 왼쪽 보기
        }

        if (Input.GetKey(KeyCode.D))
        {
            movement += Vector3.right;
            spriteRenderer.flipX = false;    // ⭐ 오른쪽 보기
        }

        // ⭐ 달리기 기능
        float currentMoveSpeed = moveSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentMoveSpeed = moveSpeed * 2f;
            Debug.Log("달리기 모드 활성화!");
        }

        // 실제 이동
        if (movement != Vector3.zero)
        {
            transform.Translate(movement * currentMoveSpeed * Time.deltaTime);
        }

        // 속도 파라미터 (Idle/Walk 판단)
        float currentSpeed = movement != Vector3.zero ? currentMoveSpeed : 0f;

        if (animator != null)
        {
            animator.SetFloat("Speed", currentSpeed);
        }

        // ⭐ 점프 애니메이션 (물리 점프 X, 애니만)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (animator != null)
            {
                animator.SetBool("isJumping", true);
                Debug.Log("점프!");
            }
        }
    }

    // Jump 애니메이션 끝났을 때 애니메이션 이벤트로 호출
    public void OnJumpAnimationEnd()
    {
        if (animator != null)
            animator.SetBool("isJumping", true);
    }
}
