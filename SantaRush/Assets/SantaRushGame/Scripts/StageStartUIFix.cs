using UnityEngine;

public class StageStartUIFix : MonoBehaviour
{
    void OnEnable()
    {
        // 혹시 다른 데서 꺼놨다면 다시 켬
        gameObject.SetActive(true);
    }

    void Start()
    {
        // 씬 진입 시 무조건 켬
        gameObject.SetActive(true);
        Canvas.ForceUpdateCanvases();
    }
}
