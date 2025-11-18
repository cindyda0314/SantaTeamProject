using UnityEngine;
using System.Collections.Generic;

public class EnemyDamageManager : MonoBehaviour
{
    public static EnemyDamageManager Instance;

    [System.Serializable]
    public struct EnemyData
    {
        public string enemyName; // 적의 이름 (하이어라키 창의 이름)
        public int damage;       // 깎일 HP 양
    }

    [Header("적 데미지 리스트")]
    // 여기에 gingerbread, snowman2 등을 등록합니다.
    public List<EnemyData> enemyList; 

    private Dictionary<string, int> damageDictionary = new Dictionary<string, int>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        foreach (var data in enemyList)
        {
            // 딕셔너리에 이름과 데미지 저장
            if (!damageDictionary.ContainsKey(data.enemyName))
            {
                damageDictionary.Add(data.enemyName, data.damage);
            }
        }
    }

    public int GetDamageByName(string name)
    {
        // 1. 정확히 일치하는 이름 찾기
        if (damageDictionary.ContainsKey(name))
        {
            return damageDictionary[name];
        }
        
        // 2. "snowman2(Clone)" 처럼 뒤에 복제 표시가 붙은 경우 처리
        // 등록된 이름이 포함되어 있으면 해당 데미지 리턴
        foreach (var key in damageDictionary.Keys)
        {
            if (name.Contains(key))
            {
                return damageDictionary[key];
            }
        }

        // 3. 등록 안 된 적이라면 기본 데미지 10
        Debug.LogWarning($"'{name}'의 데미지 정보가 없습니다. 기본값 10을 적용합니다.");
        return 10; 
    }
}