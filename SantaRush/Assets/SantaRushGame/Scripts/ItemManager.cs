using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;

    private int totalItemCount = 0; // 씬 전체 아이템 수 
    private int collectedItemCount = 0; // 플레이어가 먹은 개수

    void Awake()
    {
        // 싱글톤 (씬마다 새로 생기지만 중복 방지)
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // 현재 씬의 Item.cs 전부 찾기
        Item[] items = Object.FindObjectsByType<Item>(FindObjectsSortMode.None);
        totalItemCount = items.Length;
        collectedItemCount = 0;

        Debug.Log($"[ItemManager] 아이템 총 개수: {totalItemCount}");
    }

    public void CollectItem()
    {
        collectedItemCount++;
        Debug.Log($"[Item] {collectedItemCount}/{totalItemCount} 수집됨");

        if (collectedItemCount == totalItemCount)
        {
            Debug.Log("🎉 모든 아이템 수집 완료 → 미션 성공!");
            MissionSuccess();
        }
    }

    public void MissionSuccess()
    {
        // TODO : 미션 성공 UI 또는 다음 씬 이동
        // SceneManager.LoadScene("NextStage");
    }

    public void MissionFail()
    {
        // TODO : 미션 실패 UI 또는 재시작
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("❌ 미션 실패!");
    }

    public bool IsAllItemsCollected()
    {

    return collectedItemCount >= totalItemCount;
    
    }

}


