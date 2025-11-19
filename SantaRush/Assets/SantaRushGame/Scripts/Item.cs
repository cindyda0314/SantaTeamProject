using UnityEngine;

public class Item : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))   // 플레이어만 먹을 수 있음
        {
            ItemManager.instance.CollectItem();  // 매니저에 알림
            Destroy(gameObject);                // 아이템 삭제
        }
    }
}
