using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    [Header("BGM Clips - Main/Opening/Ending")]
    public AudioClip openingBGM; // main1, story1, story1-2
    public AudioClip endingBGM;  // story5, main2

    [Header("BGM Clips - Story (2/3/4 개별)")]
    public AudioClip story2BGM;
    public AudioClip story3BGM;
    public AudioClip story4BGM;

    [Header("BGM Clips - Stage (1/2/3/5 개별)")]
    public AudioClip stage1BGM;
    public AudioClip stage2BGM;
    public AudioClip stage3BGM;
    public AudioClip stage5BGM;

    [Header("Volumes")]
    [Range(0f, 1f)] public float openingVol = 0.35f;
    [Range(0f, 1f)] public float endingVol  = 0.35f;
    [Range(0f, 1f)] public float storyVol   = 0.30f;
    [Range(0f, 1f)] public float stageVol   = 0.30f;

    // 준비된 BGM 상태
    private AudioClip preparedClip = null;
    private float preparedVol = 0.3f;

    // 씬 로드시 자동 재생 여부
    private bool autoPlayOnLoad = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (!audioSource) audioSource = GetComponent<AudioSource>();
            if (!audioSource) audioSource = gameObject.AddComponent<AudioSource>();

            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f; // 2D
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void Start()
    {
        ApplySceneRule(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplySceneRule(scene.name);
    }

    private void ApplySceneRule(string sceneName)
    {
        PrepareBGMForScene(sceneName);

        // 무음 씬이면 정지
        if (preparedClip == null)
        {
            StopBGMNow();
            return;
        }

        // 자동재생 씬이면 바로 재생
        if (autoPlayOnLoad)
        {
            PlayPreparedBGM();
            return;
        }

        // Start 버튼에서만 재생하는 씬이면 대기(정지)
        StopBGMNow();
    }

    // 씬별 규칙 정의
    private void PrepareBGMForScene(string sceneName)
    {
        preparedClip = null;
        preparedVol = 0.3f;
        autoPlayOnLoad = false;

        switch (sceneName)
        {
            // main1: Start 버튼 누를 때만 오프닝 재생
            case "main1":
                preparedClip = openingBGM;
                preparedVol = openingVol;
                autoPlayOnLoad = false;
                break;

            // story1 / story1-2: 오프닝 유지 (자동재생)
            case "story1":
            case "story1-2":
                preparedClip = openingBGM;
                preparedVol = openingVol;
                autoPlayOnLoad = true;
                break;

            // ✅ story2/3/4 각각 다른 BGM
            case "story2":
                preparedClip = story2BGM;
                preparedVol = storyVol;
                autoPlayOnLoad = true;
                break;

            case "story3":
                preparedClip = story3BGM;
                preparedVol = storyVol;
                autoPlayOnLoad = true;
                break;

            case "story4":
                preparedClip = story4BGM;
                preparedVol = storyVol;
                autoPlayOnLoad = true;
                break;

            // ✅ stage1/2/3/5 각각 다른 BGM
            case "stage1":
                preparedClip = stage1BGM;
                preparedVol = stageVol;
                autoPlayOnLoad = true;
                break;

            case "stage2":
                preparedClip = stage2BGM;
                preparedVol = stageVol;
                autoPlayOnLoad = true;
                break;

            case "stage3":
                preparedClip = stage3BGM;
                preparedVol = stageVol;
                autoPlayOnLoad = true;
                break;

            case "stage5":
                preparedClip = stage5BGM;
                preparedVol = stageVol;
                autoPlayOnLoad = true;
                break;

            // 엔딩 자동재생
            case "story5":
            case "main2":
                preparedClip = endingBGM;
                preparedVol = endingVol;
                autoPlayOnLoad = true;
                break;

            // 기타 씬은 무음
            default:
                preparedClip = null;
                autoPlayOnLoad = false;
                break;
        }
    }

    // Start 버튼에서 호출 (main1에서 START 누를 때 등)
    public void PlayPreparedBGM()
    {
        if (!audioSource) return;

        if (preparedClip == null)
        {
            StopBGMNow();
            return;
        }

        // ✅ 같은 곡이면 유지 (Retry나 씬 재진입 때 끊김 방지)
        if (audioSource.isPlaying && audioSource.clip == preparedClip)
        {
            audioSource.volume = preparedVol;
            return;
        }

        audioSource.clip = preparedClip;
        audioSource.volume = preparedVol;
        audioSource.Play();
    }

    // 즉시 종료(별 먹을 때 등)
    public void StopBGMNow()
    {
        if (!audioSource) return;
        audioSource.Stop();
        audioSource.clip = null;
    }
}
