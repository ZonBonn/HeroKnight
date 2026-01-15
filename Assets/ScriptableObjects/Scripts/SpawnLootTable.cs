using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnLootTable", menuName = "SpawnLootTableScriptableObject/SpawnLootTable")]
public class SpawnLootTable : ScriptableObject
{
    public int level;
    
    [System.Serializable] public class KeyRate // KeyRate: từng phần tử của class này là một loại chìa khóa và tỉ lệ rơi của chìa khóa đó và GameObject để spawn ra chìa khóa đó
    {
        public Key.KeyType keyType;
        public float rate;
        public GameObject keyGameObject;
    }

    [System.Serializable] public class ClassifyEnemyType // ClassifyEnemyType: mỗi một loại enemy lại giữa một danh sách KeyRate:
    {
        public EnemyType enemyType;
        public List<KeyRate> listKeyRates;
    }

    [SerializeField] public List<ClassifyEnemyType> enemyTables; // tao một ds các list chứa dữ liệu trong này sau này thêm thì chỉ cần chỉnh đơn giản trong list này là xong
}
