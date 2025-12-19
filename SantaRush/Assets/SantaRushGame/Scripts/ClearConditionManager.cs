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
