using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance; // có một EnemyManger thôi mà đúng không ? truy cập các function không static có thể qua đây


    [SerializeField] private List<EnemyAmount> listEnemyMaximumAmount;

    [Serializable] public struct EnemyAmount // setup số lượng enemy trong map (có bao nhiêu prefab enemy thì cho bấy nhiêu vào)
    {
        public EnemyType enemyType;
        public int maxAmountEnemyType;
    }

    private Dictionary<EnemyType, int> DiedEnemyWithoutSpawnKeyAmountDic; // số lượng enemy chết mà không spawn key

    public void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        // listEnemyAmount = new List<EnemyAmount>();
        DiedEnemyWithoutSpawnKeyAmountDic = new Dictionary<EnemyType, int>();
    }

    public void Start()
    {
        
    }

    public void IncreaseEnemyDiedWithoutSpawnKeyAmount(EnemyType enemyType)
    {
        if (!DiedEnemyWithoutSpawnKeyAmountDic.ContainsKey(enemyType))
        {
            DiedEnemyWithoutSpawnKeyAmountDic[enemyType] = 0;
            return;
        }
        DiedEnemyWithoutSpawnKeyAmountDic[enemyType]++;
    }

    public int GetEnemyDiedWithoutSpawnKeyAmount(EnemyType enemyType)
    {
        if (!DiedEnemyWithoutSpawnKeyAmountDic.ContainsKey(enemyType))
        {
            return 0;
        }
        return DiedEnemyWithoutSpawnKeyAmountDic[enemyType];
    }

    public void DebugEnemyDiedWithoutSpawnKeyAmount(EnemyType enemyType)
    {
        Debug.Log(enemyType + " Died:" + DiedEnemyWithoutSpawnKeyAmountDic[enemyType]);
    }

    public List<EnemyAmount> getListEnemyMaximumAmount()
    {
        return listEnemyMaximumAmount;
    }
}
