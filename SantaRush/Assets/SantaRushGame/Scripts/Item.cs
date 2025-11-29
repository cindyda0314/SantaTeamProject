using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemKind
    {
        Ornament,
        Christmasstocking,
        Wreath
    }

    [Header("아이템 종류 선택")]
    public ItemKind kind;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어만 먹을 수 있음 (Tag = Player)
        if (!other.CompareTag("Player"))
            return;

        // 1) 소리 재생 — PlayerSound 찾아서 종류에 따라 호출
        PlayerSound sound = other.GetComponent<PlayerSound>();
        if (sound != null)
        {
            switch (kind)
            {
                case ItemKind.Ornament:
                    sound.ItemOrnament();
                    break;

                case ItemKind.Christmasstocking:
                    sound.ItemChristmasstocking();
                    break;

                case ItemKind.Wreath:
                    sound.ItemWreath();
                    break;
            }
        }

        // 2) ItemManager에 알리기 (개수 증가)
        if (ItemManager.instance != null)
        {
            ItemManager.instance.CollectItem();
        }

        // 3) 아이템 제거
        Destroy(gameObject);
    }
}
