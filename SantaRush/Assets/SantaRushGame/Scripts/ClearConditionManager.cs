using UnityEngine;

public class ClearConditionManager : MonoBehaviour
{
    public static ClearConditionManager I;

    [Header("씬에 존재하는 아이템 총 개수(자동 계산)")]
    public int totalItems;

    [Header("현재 먹은 아이템 개수")]
    public int collectedItems;

    void Awake()
    {
        if (I == null) I = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // ✅ 씬 안의 일반 아이템 개수 자동 계산 (태그로 구분)
        // 아이템 오브젝트들 Tag를 "Item"으로 맞춰두면 자동으로 카운트됨
        totalItems = GameObject.FindGameObjectsWithTag("Item").Length;
        collectedItems = 0;
    }

    public void OnItemCollected()
    {
        collectedItems++;
        Debug.Log($"아이템 획득: {collectedItems}/{totalItems}");
    }

    public bool AllItemsCollected()
    {
        return collectedItems >= totalItems;
    }
}
