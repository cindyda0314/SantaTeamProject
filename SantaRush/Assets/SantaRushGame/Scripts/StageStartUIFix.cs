using UnityEngine;

public class StageStartUIFix : MonoBehaviour
{
    void OnEnable()
    {
        
        gameObject.SetActive(true);
    }

    void Start()
    {
        // 씬 진입 시 무조건 켬
        gameObject.SetActive(true);
        Canvas.ForceUpdateCanvases();
    }
}
