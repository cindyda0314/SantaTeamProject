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

    void Awake()
    {
        // 싱글톤 + 중복 제거
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 씬 전환 시에도 파괴되지 않고 유지
        DontDestroyOnLoad(gameObject);

        // 씬 로드 이벤트 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        // 씬 로드 이벤트 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        
        // GameObject.Find가 씬 로드 직후 실패하는 것을 방지
        StartCoroutine(SetupAfterDelay());
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 로드 완료 후 1프레임 딜레이
        StartCoroutine(SetupAfterDelay());
    }
    
    private IEnumerator SetupAfterDelay()
    {
        yield return null; // 1프레임 대기
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
            else Debug.LogWarning($"[CutsceneController] sceneFlow에 현재 씬({now})이 없습니다. 씬 전환 시 에러가 발생할 수 있습니다.");
        }

        // 2) FadePanel 자동 연결 
        TryAutoFindFadePanel();

        // 3) 검은 화면 고정 방지: 항상 씬 진입 시 페이드 인
        FadeIn();

        // 4) (선택) 스토리 자동 넘김
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

        // 못 찾았으면 경고만 
        fadePanel = null;
        Debug.LogWarning($"[CutsceneController] {fadePanelObjectName}을(를) 찾지 못했습니다. (페이드 없이 전환될 수 있음)");
    }

    private void FadeIn()
    {
        if (fadePanel == null) return;

        fadePanel.DOKill(true);
        fadePanel.raycastTarget = false; // 마우스 클릭 방지 해제

       
        fadePanel.color = new Color(0, 0, 0, 1f);
        fadePanel.DOFade(0f, fadeTime).SetUpdate(true);
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

        // FadePanel이 없다면 페이드 없이 바로 로드
        if (fadePanel == null)
        {
            SceneManager.LoadScene(sceneName);
            return;
        }

        fadePanel.DOKill(true);
        fadePanel.raycastTarget = true; // 페이드 중 사용자 입력 방지

        
        fadePanel.DOFade(1f, fadeTime)
            .SetUpdate(true)
            .OnComplete(() => SceneManager.LoadScene(sceneName));
    }

    public void LoadScene(string sceneName)
    {
        FadeOutAndLoad(sceneName);
    }

    public void LoadNextScene()
    {
        if (sceneFlow == null || sceneFlow.Count == 0)
        {
            Debug.LogError("[CutsceneController] sceneFlow가 비어있습니다! Inspector/코드에서 채워주세요.");
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
}