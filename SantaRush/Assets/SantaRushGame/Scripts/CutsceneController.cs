using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class CutsceneController : MonoBehaviour
{
    [Header("페이드용 패널 (Canvas 안의 Image)")]
    public Image fadePanel;

    [Header("페이드 시간(초)")]
    public float fadeTime = 1f;

    [Header("(stage → story → stage → story ...)")]
    public List<string> sceneFlow = new List<string>();

    [Header("현재 씬 인덱스 (자동 설정을 권장)")]
    public int currentIndex = 0;

    [Header("자동으로 다음 씬으로 갈지? (스토리 씬용)")]
    public bool autoGoNext = false;

    [Header("자동 전환 딜레이(초)")]
    public float autoDelay = 3f;

    [Header("자동으로 currentIndex를 현재 씬 이름으로 맞출지")]
    public bool autoSetIndexBySceneName = true;

    private bool isLoading = false;

    void Start()
    {
        // 1) currentIndex 자동 설정(추천)
        if (autoSetIndexBySceneName && sceneFlow != null && sceneFlow.Count > 0)
        {
            string now = SceneManager.GetActiveScene().name;
            int idx = sceneFlow.IndexOf(now);
            if (idx >= 0) currentIndex = idx;
            else Debug.LogWarning($"[CutsceneController] sceneFlow에 현재 씬({now})이 없습니다. currentIndex={currentIndex} 그대로 사용합니다.");
        }

        // 2) 페이드 인(검정 → 투명)
        if (fadePanel != null)
        {
            fadePanel.raycastTarget = true; // 전환 중 입력 막기(선택)
            fadePanel.color = new Color(0, 0, 0, 1f);
            fadePanel.DOFade(0f, fadeTime).OnComplete(() =>
            {
                // 평상시엔 입력 막지 않게
                fadePanel.raycastTarget = false;
            });
        }
        else
        {
            Debug.LogWarning("[CutsceneController] fadePanel이 연결되지 않았습니다. 페이드 없이 씬 전환됩니다.");
        }

        // 3) 스토리 씬 자동 전환
        if (autoGoNext)
        {
            Invoke(nameof(LoadNextScene), autoDelay);
        }
    }

    /// <summary>
    /// 원하는 씬으로 이동(페이드 아웃 후 로드)
    /// </summary>
    public void LoadScene(string sceneName)
    {
        if (isLoading) return;
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("[CutsceneController] LoadScene 호출됐지만 sceneName이 비어있습니다.");
            return;
        }

        isLoading = true;

        // 페이드 패널이 없으면 즉시 이동
        if (fadePanel == null)
        {
            SceneManager.LoadScene(sceneName);
            return;
        }

        fadePanel.raycastTarget = true;

        // 투명 → 검정
        fadePanel.DOFade(1f, fadeTime)
            .OnComplete(() =>
            {
                SceneManager.LoadScene(sceneName);
            });
    }

    /// <summary>
    /// sceneFlow 기준으로 다음 씬으로 이동
    /// </summary>
    public void LoadNextScene()
    {
        if (isLoading) return;

        // ✅ NullReference 방지: sceneFlow 비어있으면 여기서 끝
        if (sceneFlow == null || sceneFlow.Count == 0)
        {
            Debug.LogError("[CutsceneController] sceneFlow가 비어있습니다! Inspector에서 Scene Flow를 채워주세요.");
            return;
        }

        int nextIndex = currentIndex + 1;

        // 마지막이면 더 이상 없음
        if (nextIndex < 0 || nextIndex >= sceneFlow.Count)
        {
            Debug.LogError($"[CutsceneController] 다음 씬이 없습니다. currentIndex={currentIndex}, sceneFlow.Count={sceneFlow.Count}");
            return;
        }

        string nextScene = sceneFlow[nextIndex];

        if (string.IsNullOrWhiteSpace(nextScene))
        {
            Debug.LogError($"[CutsceneController] sceneFlow[{nextIndex}]가 비어있습니다. 씬 이름을 정확히 입력하세요.");
            return;
        }

        LoadScene(nextScene);
    }

    /// <summary>
    /// (선택) 특정 인덱스로 강제 이동하고 싶을 때
    /// </summary>
    public void SetIndex(int index)
    {
        currentIndex = index;
    }
}

