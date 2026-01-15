using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using System;

public class EnemyItemsHolder : MonoBehaviour
{
    [SerializeField] private List<GameObject> fixedItems;
    // [SerializeField] private List<GameObject> randomItems;

    [SerializeField] SpawnLootTable spawnLootTableLevel;
    private HealthHandler healthHandler;
    private HealthSystem healthSystem;

    private Enemy enemy;

    public static EventHandler<DropKeyEventArgs> OnTriggerDropKey;// vì drop thì mọi enemy drop thì đều phải tăng số lượnng key đã sỉnh ra thì cái này nên thuộc về class
    public class DropKeyEventArgs : EventArgs
    {
        public DropKeyEventArgs(Key.KeyType keyType)
        {
            this.keyType = keyType;
            
        }

        public Key.KeyType keyType;
    }

    Dictionary<EnemyType, List<SpawnLootTable.KeyRate>> LootEnemyDict = new Dictionary<EnemyType, List<SpawnLootTable.KeyRate>>();
    // Dictionary<Key.KeyType, float> dropRateForEachKey = new Dictionary<Key.KeyType, float>();
    Dictionary<EnemyType, int> maximumEnemyInLevel = new Dictionary<EnemyType, int>();

    private void Awake()
    {
        enemy = gameObject.GetComponent<Enemy>();
        healthHandler = gameObject.GetComponent<HealthHandler>();
        healthSystem = healthHandler.GetHealthSystem();

        for(int i = 0 ; i < spawnLootTableLevel.enemyTables.Count ; i++)
        {
            LootEnemyDict.Add(spawnLootTableLevel.enemyTables[i].enemyType, spawnLootTableLevel.enemyTables[i].listKeyRates);

            // for(int j = 0 ; j < spawnLootTableLevel.enemyTables[i].listKeyRates.Count ; j++)
            // {
            //     dropRateForEachKey.Add(spawnLootTableLevel.enemyTables[i].listKeyRates[j].keyType, spawnLootTableLevel.enemyTables[i].listKeyRates[j].rate);
            // }
        }
        
        List<EnemyManager.EnemyAmount> listEnemeyMaximumAmount = EnemyManager.Instance.getListEnemyMaximumAmount();
        for(int i = 0 ; i < listEnemeyMaximumAmount.Count ; i++)
        {
            maximumEnemyInLevel[listEnemeyMaximumAmount[i].enemyType] = listEnemeyMaximumAmount[i].maxAmountEnemyType;
        }
    }

    private void Start()
    {
        healthSystem.OnTriggerHealthBarAsZero += OnTriggerSpawnItems;
    }

    public void OnTriggerSpawnItems()
    {
        // Spawn đồ trong người enemy của fixed Items
        for(int i = 0 ; i < fixedItems.Count ; i++)
        {
            Key key = fixedItems[i].GetComponent<Key>();
            Key.KeyType keyType = key.GetKeyType();

            if(!KeyManager.IsCanSpawnKey(keyType)) continue; // không thể spawn key
            
            GameObject item = Instantiate(fixedItems[i], gameObject.transform.position, Quaternion.identity);
            EffectDropItemsVertical(item); // dùng item chứ không phải là fixedItems[i] vì item là GameObject còn fixedItems[i] là PF

            // lấy và truyền tham số cho EventArgs
            OnTriggerDropKey?.Invoke(this, new DropKeyEventArgs(keyType));

            // chưa cho nhặt key vội hãy để tầm 1s sau mới cho nhặt
            SetDoNotAllowedPickKeyAfter1Seconds(item);
        }

        // Spawn đồ trong người enemy của random Items (lấy ở Spawn Loot Table)
        List<SpawnLootTable.KeyRate> listKeyRate = TakeRatebyEnemyType(enemy.enemyType);
        if(listKeyRate != null)
        {
            List<int> randomListIdx = RandomIDX(listKeyRate.Count);
            for(int j = 0 ; j < randomListIdx.Count ; j++)
            {
                // Debug.Log("randomListIdx thứ " + j + " là:" + randomListIdx[j]);
                
                Key key = listKeyRate[randomListIdx[j]].keyGameObject.GetComponent<Key>();
                Key.KeyType keyType = key.GetKeyType();
                if(KeyManager.IsCanSpawnKey(keyType) == true) // nếu sl vẫn chưa vượt quá số lượng cho phép
                {
                    bool ShouldDropKeyVar = ShouldDropKey(listKeyRate[randomListIdx[j]].rate, keyType);
                    if(ShouldDropKeyVar == true) // và thêm cả tỉ lệ cũng ra thì cho phép
                    {
                        Debug.Log("Spawn: " + listKeyRate[randomListIdx[j]].keyGameObject);

                        GameObject item = Instantiate(listKeyRate[randomListIdx[j]].keyGameObject, gameObject.transform.position, Quaternion.identity);
                        EffectDropItemsVertical(item);

                        // lấy và truyền tham số cho EventArgs
                        // Key.KeyType keyType = item.GetComponent<Key.KeyType>(); 
                        OnTriggerDropKey?.Invoke(this, new DropKeyEventArgs(keyType));

                        // chưa cho nhặt key vội hãy để tầm 1s sau mới cho nhặt
                        SetDoNotAllowedPickKeyAfter1Seconds(item);

                        break;
                    }
                    else
                    {
                        // cộng thêm số lượng rate for key vì enemy chết rồi mà không spawn key tại đây(lấy enemyType thông qua Enemy thôi)
                        // tại sao cái này lại chạy đúng ? rất có thể nó sẽ rơi vòng 2 lần for và sẽ ++ lên 2 dù chỉ kill 1 enemy
                        EnemyManager.Instance.IncreaseEnemyDiedWithoutSpawnKeyAmount(enemy.enemyType); 
                        
                        Debug.Log("Không spawn key được vì KHÔNG GACHA ra được: " + listKeyRate[randomListIdx[j]].keyGameObject);
                        // Debug.Log("Không spawn key được vì không gacha ra được");
                    }
                }
                else
                {
                    // nếu vượt quá số lượng cho phép rồi thì tăng IncreaseEnemyDiedWithoutSpawnKeyAmount() làm gì nữa
                    Debug.Log("Không spawn key được vì QUÁ SỐ LƯỢNG rồi" + listKeyRate[randomListIdx[j]].keyGameObject);
                }
            }
        }
        else
        {
            Debug.Log("listKeyRate is null");
        }
    }

    private void EffectDropItemsHorizontal(GameObject items)
    {
        Rigidbody2D itemsRB2D = items.GetComponent<Rigidbody2D>();
        if(itemsRB2D != null)
        {
            float xForce = UnityEngine.Random.Range(-1f, 1f);
            itemsRB2D.linearVelocity = new Vector2(xForce, itemsRB2D.linearVelocityY);
        }
    }

    private void EffectDropItemsVertical(GameObject items)
    {
        Rigidbody2D itemRB2D = items.GetComponent<Rigidbody2D>();
        if(itemRB2D != null)
        {
            float launchForceX = 1;
            float launchForceY = 1f;
            itemRB2D.linearVelocity = new Vector3(UnityEngine.Random.Range(-launchForceX, +launchForceX), launchForceY);
        }
    }

    // CONTINUE YOUR WORK IN HERE (FIX NỐT HÀM DƯỚI ĐÂY ớ chat gpt đoạn chat đầu tiên tin nhắn đầu tiên)
    public bool ShouldDropKey(float dropRate, Key.KeyType keyType) // hàm drop theo tỉ lệ
    {
        float randomRate = UnityEngine.Random.Range(0f, 1f);
        // ở đây sẽ cộng thêm tỉ lệ enemy đã chết mà không drop key thì sẽ tăng tỉ lệ 
        float defaultValueKey = getKeyRateByEnemyTypeAndKeyType(enemy.enemyType, keyType);
        float rateForEachEnemy = (1 - defaultValueKey) / maximumEnemyInLevel[enemy.enemyType];
        float addlyRate = EnemyManager.Instance.GetEnemyDiedWithoutSpawnKeyAmount(enemy.enemyType) * rateForEachEnemy;

        if(randomRate - addlyRate < dropRate) // bé hơn cả tỉ lệ rơi
        {
            return true;
        }
        return false;
    }

    // viết một hàm truyền vào một số n rồi trả ra List random idx từ 0 -> n-1;
    private List<int> RandomIDX(int n)
    {
        List<int> list = new List<int>(n);
        
        for (int i = 0; i < n; i++)
            list.Add(i);

        for (int i = n - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }

        return list;
    }

    private List<SpawnLootTable.KeyRate> TakeRatebyEnemyType(EnemyType enemyType)
    {
        if(LootEnemyDict.TryGetValue(enemyType, out List<SpawnLootTable.KeyRate> listBack))
        {
            return listBack;
        }
        return null;
    }

    private void SetDoNotAllowedPickKeyAfter1Seconds(GameObject item)
    {
        Key key = item.GetComponent<Key>();
        if(key != null)
        {
            FunctionTimer.Create(key.SetCanPickUpTrue, 1f);
        }
    }

    public float getKeyRateByEnemyTypeAndKeyType(EnemyType enemyType, Key.KeyType keyType)
    {
        List<SpawnLootTable.KeyRate> listKeyRate =  LootEnemyDict[enemyType];
        for(int j = 0 ; j < listKeyRate.Count ; j++)
        {
            if(keyType == listKeyRate[j].keyType)
            {
                return listKeyRate[j].rate;
            }
        }
        return 0;
    }
}
