using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class CutsceneController : MonoBehaviour
{
    public static CutsceneController Instance { get; private set; }

    [Header("페이드 패널 이름(씬마다 동일하게)")]
    public string fadePanelObjectName = "FadePanel";

    [Header("페이드용 패널(Image) - 씬마다 자동 재연결")]
    public Image fadePanel;

    [Header("페이드 시간(초)")]
    public float fadeTime = 1f;

    [Header("씬 진행 순서 (main → story → stage → story ...)")]
    public List<string> sceneFlow = new List<string>()
    {
        "main1",
        "main2",
        "story1",
        "story1-2",
        "stage1",
        "story2",
        "stage2",
        "story3",
        "stage3",
        "story4",
        "stage5",
        "story5"
    };

    [Header("스토리 씬 목록(자동 진행용)")]
    public List<string> storyScenes = new List<string>()
    {
        "story1",
        "story1-2",
        "story2",
        "story3",
        "story4",
        "story5"
    };

    [Header("스토리 자동 전환 딜레이(초) - ImageSequence를 안 쓸 때만")]
    public float autoDelay = 0f;

    [Header("현재 씬 인덱스 자동 맞춤")]
    public bool autoSetIndexBySceneName = true;

    private int currentIndex = 0;

   
    private bool isLoading = false;
    private Tween fadeTween = null;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (Application.isPlaying)
            DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        
        KillFadeTween();
    }

    void Start()
    {
        StartCoroutine(SetupAfterDelay());
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(SetupAfterDelay());
    }

    private IEnumerator SetupAfterDelay()
    {
        yield return null;
        SetupForCurrentScene();
    }

    private void SetupForCurrentScene()
    {
        isLoading = false;

        // 1) 현재 씬 인덱스 자동 설정
        if (autoSetIndexBySceneName && sceneFlow != null && sceneFlow.Count > 0)
        {
            string now = SceneManager.GetActiveScene().name;
            int idx = sceneFlow.IndexOf(now);
            if (idx >= 0) currentIndex = idx;
            else Debug.LogWarning($"[CutsceneController] sceneFlow에 현재 씬({now})이 없습니다.");
        }

        // 2) FadePanel 자동 연결
        TryAutoFindFadePanel();

        // 3) 씬 진입 시 페이드 인
        FadeIn();

        // 4) 혹시 입력 막고 있으면 해제
        if (fadePanel != null) fadePanel.raycastTarget = false;

        // 5) (선택) 스토리 자동 넘김
        string nowScene = SceneManager.GetActiveScene().name;
        if (autoDelay > 0f && storyScenes.Contains(nowScene))
        {
            CancelInvoke(nameof(LoadNextScene));
            Invoke(nameof(LoadNextScene), autoDelay);
        }
        else
        {
            CancelInvoke(nameof(LoadNextScene));
        }
    }

    private void TryAutoFindFadePanel()
    {
        
        fadePanel = null;

        var go = GameObject.Find(fadePanelObjectName);
        if (go != null)
        {
            var img = go.GetComponent<Image>();
            if (img != null)
            {
                fadePanel = img;
                return;
            }
        }

        // 못 찾았으면 경고
        Debug.LogWarning($"[CutsceneController] {fadePanelObjectName}을(를) 찾지 못했습니다. (페이드 없이 전환)");
    }

    
    private void KillFadeTween()
    {
        if (fadeTween != null && fadeTween.IsActive())
        {
            fadeTween.Kill(false);
        }
        fadeTween = null;

        // fadePanel 자체에 걸려있던 트윈도 정리 (안전)
        if (fadePanel != null)
        {
            // DOTween이 target으로 관리하는 트윈까지 싹 정리
            DOTween.Kill(fadePanel, false);
            fadePanel.DOKill(false);
        }
    }

    private void FadeIn()
    {
        if (fadePanel == null) return;

        KillFadeTween();

        fadePanel.raycastTarget = false;
        fadePanel.color = new Color(0, 0, 0, 1f);

        fadeTween = fadePanel
            .DOFade(0f, fadeTime)
            .SetUpdate(true)
            .SetLink(fadePanel.gameObject, LinkBehaviour.KillOnDestroy); 
    }

    private void FadeOutAndLoad(string sceneName)
    {
        if (isLoading) return;
        isLoading = true;

        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("[CutsceneController] 이동할 씬 이름이 비어있습니다.");
            isLoading = false;
            return;
        }

        // 씬 전환 직전에 혹시 자동 Invoke 걸린 것 있으면 취소
        CancelInvoke(nameof(LoadNextScene));

        // FadePanel 없으면 바로 로드
        if (fadePanel == null)
        {
            SceneManager.LoadScene(sceneName);
            return;
        }

        KillFadeTween();

        fadePanel.raycastTarget = true;

        fadeTween = fadePanel
            .DOFade(1f, fadeTime)
            .SetUpdate(true)
            .SetLink(fadePanel.gameObject, LinkBehaviour.KillOnDestroy) 
            .OnComplete(() =>
            {
                // OnComplete 시점에 객체가 살아있지 않을 수도 있으니 방어
                SceneManager.LoadScene(sceneName);
            });
    }

    // -------------------------
    // 외부 호출
    // -------------------------
    public void LoadScene(string sceneName)
    {
        FadeOutAndLoad(sceneName);
    }

    public void LoadNextScene()
    {
        if (sceneFlow == null || sceneFlow.Count == 0)
        {
            Debug.LogError("[CutsceneController] sceneFlow가 비어있습니다!");
            return;
        }

        int nextIndex = currentIndex + 1;
        if (nextIndex < 0 || nextIndex >= sceneFlow.Count)
        {
            Debug.LogError($"[CutsceneController] 다음 씬이 없습니다. currentIndex={currentIndex}, count={sceneFlow.Count}");
            return;
        }

        string nextScene = sceneFlow[nextIndex];
        FadeOutAndLoad(nextScene);
    }

    public void RestartFlowFromStory1()
    {
        int idx = sceneFlow.IndexOf("story1");
        if (idx < 0)
        {
            Debug.LogError("[CutsceneController] sceneFlow에 story1이 없습니다!");
            return;
        }

        isLoading = false;
        currentIndex = idx;

        FadeOutAndLoad("story1");
    }

    public void StartFromMain()
    {
        RestartFlowFromStory1();
    }

    public void GoToMain()
    {
        isLoading = false;
        FadeOutAndLoad("main1");
    }
}
