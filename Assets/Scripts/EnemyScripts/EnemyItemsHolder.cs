using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using System;

public class EnemyItemsHolder : MonoBehaviour
{
    [SerializeField] private List<GameObject> fixedItems;
    [SerializeField] private List<GameObject> randomItems;

    [SerializeField] SpawnLootTable spawnLootTableLevel;
    private HealthHandler healthHandler;
    private HealthSystem healthSystem;

    private Enemy enemy;

    public static Action OnDropKey;// vì drop thì mọi enemy drop thì đều phải tăng số lượnng key đã sỉnh ra thì cái này nên thuộc về class

    Dictionary<EnemyType, List<SpawnLootTable.KeyRate>> LootEnemyDict = new Dictionary<EnemyType, List<SpawnLootTable.KeyRate>>();

    private void Awake()
    {
        enemy = gameObject.GetComponent<Enemy>();
        healthHandler = gameObject.GetComponent<HealthHandler>();
        healthSystem = healthHandler.GetHealthSystem();

        for(int i = 0 ; i < spawnLootTableLevel.enemyTables.Count ; i++)
        {
            LootEnemyDict.Add(spawnLootTableLevel.enemyTables[i].enemyType, spawnLootTableLevel.enemyTables[i].listKeyRates);
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
            GameObject item = Instantiate(fixedItems[i], gameObject.transform.position, Quaternion.identity);
            EffectDropItemsVertical(item); // dùng item chứ không phải là fixedItems[i] vì item là GameObject còn fixedItems[i] là PF

            OnDropKey?.Invoke();

            // chưa cho nhặt key vội hãy để tầm 1s sau mới cho nhặt
            SetDoNotAllowedPickKeyAfter1Seconds(item);
        }

        // Spawn đồ trong người enemy của random Items
        List<SpawnLootTable.KeyRate> listKeyRate = TakeRatebyEnemyType(enemy.enemyType);
        if(listKeyRate != null)
        {
            List<int> randomListIdx = RandomIDX(listKeyRate.Count);
            for(int j = 0 ; j < randomListIdx.Count ; j++)
            {
                Debug.Log("randomListIdx thứ " + j + " là:" + randomListIdx[j]);

                bool ShouldDropKeyVar = ShouldDropKey(listKeyRate[randomListIdx[j]].rate);
                if(ShouldDropKeyVar == true)
                {
                    Debug.Log("Spawn: " + listKeyRate[randomListIdx[j]].keyGameObject);

                    GameObject item = Instantiate(listKeyRate[randomListIdx[j]].keyGameObject, gameObject.transform.position, Quaternion.identity);
                    EffectDropItemsVertical(item);

                    OnDropKey?.Invoke();

                    // chưa cho nhặt key vội hãy để tầm 1s sau mới cho nhặt
                    SetDoNotAllowedPickKeyAfter1Seconds(item);

                    break;
                }
                else
                {
                    Debug.Log("Không spawn: " + listKeyRate[randomListIdx[j]].keyGameObject);
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

    

    public bool ShouldDropKey(float dropRate)
    {
        float randomRate = UnityEngine.Random.Range(0f, 1f);
        if(randomRate < dropRate) // bé hơn cả tỉ lệ rơi
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
}
