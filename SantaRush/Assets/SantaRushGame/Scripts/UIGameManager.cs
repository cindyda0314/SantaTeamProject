using UnityEngine;
using UnityEngine.SceneManagement;

public class UIGameManager : MonoBehaviour
{
    [Header("UI 패널")]
    public GameObject titleScreenPanel;   // Panel
    public GameObject gameOverPanel;      // Panel2

    void Start()
    {
        // 처음 실행인지 체크
        bool isFirstStart = PlayerPrefs.GetInt("FirstStart", 1) == 1;

        if (isFirstStart)
        {
            // 처음 시작일 때만 타이틀 패널 표시
            titleScreenPanel.SetActive(true);
            gameOverPanel.SetActive(false);
            Time.timeScale = 0f;
        }
        else
        {
            // Retry로 돌아온 경우 바로 게임 시작
            titleScreenPanel.SetActive(false);
            gameOverPanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    // Start 버튼
    public void StartGame()
    {
        PlayerPrefs.SetInt("FirstStart", 0);
        titleScreenPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    // 게임오버 패널 표시
    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // Retry 버튼
    public void RetryGame()
    {
        PlayerPrefs.SetInt("FirstStart", 0); // 다시 시작 = 처음 아님
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
