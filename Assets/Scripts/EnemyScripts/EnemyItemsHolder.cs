using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;

public class EnemyItemsHolder : MonoBehaviour
{
    [SerializeField] private List<GameObject> fixedItems;
    [SerializeField] private List<GameObject> randomItems;

    [SerializeField] SpawnLootTable spawnLootTableLevel1;
    private HealthHandler healthHandler;
    private HealthSystem healthSystem;

    private Enemy enemy;

    private void Awake()
    {
        enemy = gameObject.GetComponent<Enemy>();
        healthHandler = gameObject.GetComponent<HealthHandler>();
        healthSystem = healthHandler.GetHealthSystem();
    }

    private void Start()
    {
        healthSystem.OnTriggerHealthBarAsZero += OnTriggerSpawnItems;
    }

    private void Update()
    {
        
    }

    public void OnTriggerSpawnItems()
    {
        for(int i = 0 ; i < fixedItems.Count ; i++)
        {
            Instantiate(fixedItems[i], gameObject.transform.position, Quaternion.identity);
            EffectDropItemsVertical(fixedItems[i]);
        }

        for(int i = 0 ; i < spawnLootTableLevel1.enemyTables.Count ; i++)
        {
            if(enemy.enemyType == spawnLootTableLevel1.enemyTables[i].enemyType)
            {
                // random idx for iterator
                List<int> randomListIdx = RandomIDX(spawnLootTableLevel1.enemyTables[i].listKeyRates.Count);
                for(int j = 0 ; j < randomListIdx.Count ; j++)
                {
                    Debug.Log("randomListIdx thứ " + j + " là:" + randomListIdx[j]);
                    bool ShouldDropKeyVar = ShouldDropKey(spawnLootTableLevel1.enemyTables[i].listKeyRates[randomListIdx[j]].rate);
                    if(ShouldDropKeyVar == true)
                    {
                        Debug.Log("Spawn: " + spawnLootTableLevel1.enemyTables[i].listKeyRates[randomListIdx[j]].keyGameObject);
                        Instantiate(spawnLootTableLevel1.enemyTables[i].listKeyRates[randomListIdx[j]].keyGameObject, gameObject.transform.position, Quaternion.identity);
                        EffectDropItemsVertical(spawnLootTableLevel1.enemyTables[i].listKeyRates[randomListIdx[j]].keyGameObject);
                        break;
                    }
                    else
                    {
                        Debug.Log("Không spawn: " + spawnLootTableLevel1.enemyTables[i].listKeyRates[randomListIdx[j]].keyGameObject);
                    }
                }
            }
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
        if(randomRate < dropRate)
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
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }

        return list;
    }
}
