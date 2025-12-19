using UnityEngine;
using UnityEngine.UI;

public class ImageSequence : MonoBehaviour
{
    public Image display;
    public Sprite[] images;

    public float delay = 2f;    // 이미지 전환 간격
    private int index = 0;
    private float timer = 0f;
 
    private bool finished = false; 


    void Start()
    {
        if (images.Length > 0)
            display.sprite = images[0];
    }

    void Update()
    {
        if (finished) return; 

        timer += Time.deltaTime;

        if (timer >= delay)
        {
            timer = 0f;
            index++;

            if (index < images.Length)
            {
                // 다음 이미지 표시
                display.sprite = images[index];
            }
            else
            {
                // 모든 이미지 표시 완료
                finished = true;
                Debug.Log("🎬 컷신 끝 → CutsceneController 호출");

             
                if (CutsceneController.Instance != null)
                {
                    CutsceneController.Instance.LoadNextScene();
                }
                else
                {
                    Debug.LogError("[ImageSequence] CutsceneController 인스턴스를 찾을 수 없어 씬 전환 실패.");
                }
            }
        }
    }
}