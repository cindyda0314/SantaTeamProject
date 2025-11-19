using UnityEngine;
using System.Collections.Generic;

public class DamageManager : MonoBehaviour
{
    public static DamageManager Instance;

    [System.Serializable]
    public struct DamageData
    {
        public string objectName;  // 적 또는 장애물 이름
        public int damage;
    }

    [Header("적 데미지 리스트")]
    public List<DamageData> enemyList = new List<DamageData>();

    [Header("장애물 데미지 리스트")]
    public List<DamageData> obstacleList = new List<DamageData>();

    private Dictionary<string, int> damageDictionary = new Dictionary<string, int>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // 적 + 장애물 통합 등록
        AddListToDictionary(enemyList);
        AddListToDictionary(obstacleList);
    }

    private void AddListToDictionary(List<DamageData> list)
    {
        foreach (var data in list)
        {
            if (!damageDictionary.ContainsKey(data.objectName))
                damageDictionary.Add(data.objectName, data.damage);
        }
    }

    public int GetDamageByName(string name)
    {
        // 정확히 일치
        if (damageDictionary.ContainsKey(name))
            return damageDictionary[name];

        // Clone 호환
        foreach (var key in damageDictionary.Keys)
        {
            if (name.Contains(key))
                return damageDictionary[key];
        }

        Debug.LogWarning($"'{name}' 데미지 정보 없음 → 기본 10 적용");
        return 10;
    }
}
