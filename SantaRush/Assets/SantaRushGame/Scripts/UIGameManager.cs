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
        // 씬이 바뀔 때 isRetry를 자동으로 초기화하기 위함
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }

    private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        if (oldScene.buildIndex != newScene.buildIndex)
        {
            isRetry = false;
        }
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

    // Start 버튼
    public void StartGame()
    {
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
        isRetry = false; // 메인으로 갈 땐 항상 초기화
        Time.timeScale = 1f;
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
