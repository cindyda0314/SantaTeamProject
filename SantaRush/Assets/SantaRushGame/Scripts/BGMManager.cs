using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    [Header("BGM Clips")]
    public AudioClip openingBGM; // main1, story1, story1-2
    public AudioClip scene1BGM;  // stage1
    public AudioClip scene2BGM;  // stage2
    public AudioClip scene3BGM;  // stage3
    public AudioClip scene5BGM;  // stage5
    public AudioClip endingBGM;  // story5, main2

    [Header("Volumes")]
    [Range(0f, 1f)] public float openingVol = 0.35f;
    [Range(0f, 1f)] public float scene1Vol  = 0.30f;
    [Range(0f, 1f)] public float scene2Vol  = 0.30f;
    [Range(0f, 1f)] public float scene3Vol  = 0.30f;
    [Range(0f, 1f)] public float scene5Vol  = 0.30f;
    [Range(0f, 1f)] public float endingVol  = 0.35f;

    // "Start 누르면 재생"용 준비 상태
    private AudioClip preparedClip = null;
    private float preparedVol = 0.3f;

    // ✅ 씬 로드시 자동 재생할지 여부
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

        // 무음 씬이면 무조건 정지
        if (preparedClip == null)
        {
            StopBGMNow();
            return;
        }

        // 자동재생 씬이면: 같은 곡이면 유지, 아니면 재생
        if (autoPlayOnLoad)
        {
            PlayPreparedBGM();
            return;
        }

        // 자동재생 아니면(= Start 버튼에서만 재생): 씬 들어올 때는 일단 정지
        StopBGMNow();
    }

    // 씬별로 "재생할 곡" + "자동재생 여부" 준비
    private void PrepareBGMForScene(string sceneName)
    {
        preparedClip = null;
        preparedVol = 0.3f;
        autoPlayOnLoad = false;

        switch (sceneName)
        {
            // ✅ main1: Start 버튼 누를 때만 오프닝 재생
            case "main1":
                preparedClip = openingBGM;
                preparedVol = openingVol;
                autoPlayOnLoad = false;
                break;

            // ✅ story1, story1-2: 오프닝이 계속 이어지도록 자동재생(유지)
            case "story1":
            case "story1-2":
                preparedClip = openingBGM;
                preparedVol = openingVol;
                autoPlayOnLoad = true;
                break;

            // (필요하면 story1도 Start 버튼이 있다면 autoPlayOnLoad=false로 바꿀 수 있음)

            // ✅ stage: Start 버튼 누를 때만 재생
            case "stage1":
                preparedClip = scene1BGM;
                preparedVol = scene1Vol;
                autoPlayOnLoad = false;
                break;

            case "stage2":
                preparedClip = scene2BGM;
                preparedVol = scene2Vol;
                autoPlayOnLoad = false;
                break;

            case "stage3":
                preparedClip = scene3BGM;
                preparedVol = scene3Vol;
                autoPlayOnLoad = false;
                break;

            case "stage5":
                preparedClip = scene5BGM;
                preparedVol = scene5Vol;
                autoPlayOnLoad = false;
                break;

            // ✅ 엔딩(스토리5/메인2): 자동재생이 자연스러움
            case "story5":
            case "main2":
                preparedClip = endingBGM;
                preparedVol = endingVol;
                autoPlayOnLoad = true;
                break;

            // ✅ 무음 씬
            case "story2":
            case "story3":
            case "story4":
            default:
                preparedClip = null;
                autoPlayOnLoad = false;
                break;
        }
    }

    // Start 버튼에서 호출
    public void PlayPreparedBGM()
    {
        if (!audioSource) return;

        if (preparedClip == null)
        {
            StopBGMNow();
            return;
        }

        // ✅ 같은 곡이 이미 재생 중이면 끊지 말고 유지(볼륨만 보정)
        if (audioSource.isPlaying && audioSource.clip == preparedClip)
        {
            audioSource.volume = preparedVol;
            return;
        }

        audioSource.clip = preparedClip;
        audioSource.volume = preparedVol;
        audioSource.Play();
    }

    // 별 먹을 때 즉시 종료
    public void StopBGMNow()
    {
        if (!audioSource) return;
        audioSource.Stop();
        audioSource.clip = null;
    }
}
