using UnityEngine;

public class StarGoal : MonoBehaviour
{
    private CutsceneController cutscene;
    private bool cleared = false;

    private void Awake()
    {
        cutscene = FindFirstObjectByType<CutsceneController>();
        if (cutscene == null)
            Debug.LogError("❌ CutsceneController가 씬에 없습니다!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (cleared) return;
        if (!other.CompareTag("Player")) return;

        if (ItemManager.instance == null)
        {
            Debug.LogError("❌ ItemManager.instance가 null입니다! stage 씬에 ItemManager 오브젝트가 있어야 해요.");
            return;
        }

        Debug.Log($"[StarGoal] items: {ItemManager.instance.IsAllItemsCollected()}  " +
                  $"({GetCountDebug()})");

        if (!ItemManager.instance.IsAllItemsCollected())
        {
            Debug.Log("⚠ 아직 모든 아이템을 다 먹지 않았어요!");
            return;
        }

        cleared = true;
        gameObject.SetActive(false);

        cutscene?.LoadNextScene();
    }

    private string GetCountDebug()
    {
        // ItemManager에 프로퍼티가 없어서, 여기서는 로그용으로만 간단히 표기
        // (원하면 ItemManager에 TotalCount/CollectedCount 프로퍼티도 추가해줄게)
        return "check passed";
    }
}
