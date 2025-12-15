using UnityEngine;

public class StarGoal : MonoBehaviour
{
    private CutsceneController cutscene;
    private bool cleared = false;

    void Awake()
    {
        cutscene = Object.FindFirstObjectByType<CutsceneController>();
        if (cutscene == null)
            Debug.LogError("[StarGoal] 씬에 CutsceneController가 없습니다!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (cleared) return;
        if (!other.CompareTag("Player")) return;

        // 아직 아이템을 다 안 먹었으면 클리어 불가
        if (ItemManager.instance == null || !ItemManager.instance.IsAllItemsCollected())
        {
            Debug.Log("⚠ 아직 모든 아이템을 다 먹지 않았어요!");
            return;
        }

        cleared = true;

        // 먹는 연출
        gameObject.SetActive(false);

        // 페이드 → 다음 씬
        if (cutscene != null)
            cutscene.LoadNextScene();
        else
            Debug.LogError("[StarGoal] CutsceneController가 null이라 씬 전환 불가!");
    }
}
