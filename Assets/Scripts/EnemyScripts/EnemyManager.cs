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

    private void Start()
    {
        
    }

    private void Update()
    {
        DebugEnemyDiedWithoutSpawnKeyAmountHook();
    }

    public void IncreaseEnemyDiedWithoutSpawnKeyAmount(EnemyType enemyType)
    {
        if (!DiedEnemyWithoutSpawnKeyAmountDic.ContainsKey(enemyType))
        {
            Debug.Log("chưa có enemyType này khởi tạo đã = 1");
            DiedEnemyWithoutSpawnKeyAmountDic[enemyType] = 1; // 1 vì khi enemy truyền vào không có lúc đấy đã chết rồi mà
            return;
        }
        Debug.Log("Công thêm số lượng " + enemyType + " chết mà không rơi ra key");
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

    private void DebugEnemyDiedWithoutSpawnKeyAmountHook()
    {
        if(Input.GetKey(KeyCode.C) && Input.GetKey(KeyCode.D)  && Input.GetKey(KeyCode.L))
        {
            DebugEnemyDiedWithoutSpawnKeyAmount(EnemyType.LightEnemy);
        }
        else if(Input.GetKey(KeyCode.C) && Input.GetKey(KeyCode.D)  && Input.GetKey(KeyCode.H))
        {
            DebugEnemyDiedWithoutSpawnKeyAmount(EnemyType.HeavyEnemy);
        }
    }
    private void DebugEnemyDiedWithoutSpawnKeyAmount(EnemyType enemyType)
    {
        if (!DiedEnemyWithoutSpawnKeyAmountDic.ContainsKey(enemyType))
        {
            Debug.Log(enemyType + " Died:0");
            return;
        }
        Debug.Log(enemyType + " Died:" + DiedEnemyWithoutSpawnKeyAmountDic[enemyType]);
    }

    public List<EnemyAmount> getListEnemyMaximumAmount()
    {
        return listEnemyMaximumAmount;
    }
}
