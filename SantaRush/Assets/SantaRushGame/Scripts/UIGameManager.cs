using UnityEngine;
using UnityEngine.SceneManagement;

public class UIGameManager : MonoBehaviour
{
    private static bool isRetry = false;

    [Header("UI 패널")]
    public GameObject titleScreenPanel;   // 시작 화면(START 버튼 포함)
    public GameObject gameOverPanel;      // 게임 오버 화면
    public GameObject healthBar;          // 체력바

    [Header("추가 타이틀 패널(옵션)")]
    public GameObject titlePanel;

    void OnEnable()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }

    private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        // Retry는 "같은 씬 재시작"에서만 의미
        // 다른 씬으로 넘어가면 항상 초기화
        if (oldScene.buildIndex != newScene.buildIndex)
            isRetry = false;
    }

    void Start()
    {
        // Retry로 들어온 경우(같은 씬 재시작)
        if (isRetry)
        {
            SafeSetActive(titleScreenPanel, false);
            SafeSetActive(gameOverPanel, false);
            SafeSetActive(healthBar, true);

            Time.timeScale = 1f;
            return;
        }

        // 처음 실행한 경우(또는 다른 씬에서 넘어온 경우)
        SafeSetActive(titleScreenPanel, true);
        SafeSetActive(gameOverPanel, false);
        SafeSetActive(healthBar, false);

        Time.timeScale = 0f;
    }

    // ✅ Start 버튼 (main1에서는 "스토리 흐름 재시작", stage에서는 "게임 시작")
    public void StartGame()
    {
        string now = SceneManager.GetActiveScene().name;

        // 1) ✅ main1에서는: story1부터 시퀀스 재시작
        if (now == "main1")
        {
            Debug.Log("[UIGameManager] main1 START -> Restart flow from story1");
            Time.timeScale = 1f;

            // titleScreenPanel은 굳이 안 숨겨도 되지만, 혹시 잠깐 깜빡임 방지용으로 숨김
            SafeSetActive(titleScreenPanel, false);

            if (CutsceneController.Instance != null)
            {
                CutsceneController.Instance.RestartFlowFromStory1();
            }
            else
            {
                // 혹시 CutsceneController가 없다면 직접 이동(최후 안전장치)
                SceneManager.LoadScene("story1");
            }
            return;
        }

        // 2) ✅ stage 씬에서는: 기존대로 UI 숨기고 게임 시작
        SafeSetActive(titleScreenPanel, false);
        SafeSetActive(healthBar, true);

        Time.timeScale = 1f;
        Debug.Log("CLICK");
    }

    // 게임오버 패널 표시
    public void ShowGameOver()
    {
        SafeSetActive(gameOverPanel, true);
        Time.timeScale = 0f;
    }

    // Retry 버튼
    public void RetryGame()
    {
        isRetry = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // exit 눌렀을 때 main으로 이동
    public void GoMain()
    {
        isRetry = false;
        Time.timeScale = 1f;

        // ✅ CutsceneController가 있으면 페이드 포함해서 main1로
        if (CutsceneController.Instance != null)
            CutsceneController.Instance.GoToMain();
        else
            SceneManager.LoadScene("main1");
    }

    // titlePanel이 따로 있을 때만 사용
    public void StartStage()
    {
        SafeSetActive(titlePanel, false);
    }

    // --------------------------
    // 유틸: null 안전 활성/비활성
    // --------------------------
    private void SafeSetActive(GameObject go, bool on)
    {
        if (go != null) go.SetActive(on);
    }
}
