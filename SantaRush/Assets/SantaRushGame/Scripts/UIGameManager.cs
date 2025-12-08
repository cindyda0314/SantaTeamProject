using UnityEngine;
using UnityEngine.SceneManagement;

public class UIGameManager : MonoBehaviour
{
    private static bool isRetry = false;   // ⭐ 씬 재시작 여부 저장

    [Header("UI 패널")]
    public GameObject titleScreenPanel;   // 시작 화면
    public GameObject gameOverPanel;      // 게임 오버 화면

    void Start()
    {
        // Retry로 들어온 경우
        if (isRetry)
        {
            if (titleScreenPanel != null) titleScreenPanel.SetActive(false);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            Time.timeScale = 1f;
            return;
        }

        // 처음 실행한 경우
        if (titleScreenPanel != null) titleScreenPanel.SetActive(true);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        Time.timeScale = 0f;
    }

    // Start 버튼
    public void StartGame()
    {
        titleScreenPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    // 게임오버 패널 표시
    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        Time.timeScale = 0f;
    }

    // Retry 버튼
    public void RetryGame()
    {
        isRetry = true; // ⭐ 다음 씬 로드 때는 타이틀 안 보여주기
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //exit눌렀을 때 main으로 이동
    public void GoMain()
    {
        isRetry = false;   // 처음 화면처럼 보이게 하고 싶다면 false 유지
        Time.timeScale = 1f;
        SceneManager.LoadScene("main1");   // ← 네가 연결하려는 씬 이름
    }
}


