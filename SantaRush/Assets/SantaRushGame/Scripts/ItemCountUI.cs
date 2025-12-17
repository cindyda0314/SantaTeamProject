using TMPro;
using UnityEngine;

public class ItemCountUI : MonoBehaviour
{
    public TextMeshProUGUI itemText;

    void Update()
    {
        if (ItemManager.instance == null) return;

        itemText.text =
            $"Items {ItemManager.instance.collectedItemCount} / {ItemManager.instance.totalItemCount}";
    }
}
