using UnityEngine;
using UnityEngine.UI;

public class ImageSequence : MonoBehaviour
{
    public Image display;      
    public Sprite[] images;    
    private int index = 0;

    public float delay = 2f;   // 이미지 전환 시간 (초) ⭐ 추가
    private float timer = 0f;  // 타이머 ⭐ 추가

    void Start()
    {
        if (images.Length > 0)
            display.sprite = images[0];
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= delay) // 일정 시간 지나면 자동 전환
        {
            timer = 0f;
            index++;

            if (index < images.Length)
            {
                display.sprite = images[index];
            }
            else
            {
                Debug.Log("컷신 끝");
                // 여기서 다음 씬 이동하거나 화면 닫기 가능
            }
        }
    }
}
