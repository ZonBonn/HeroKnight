using System.Collections.Generic;
using UnityEngine;

public class ChestItemsHolder : MonoBehaviour
{
    [SerializeField] private List<GameObject> itemsInSetedChest; // đồ trong này sẽ được spawn theo thứ tự ném vào
    [SerializeField] private List<GameObject> itemsInRandomChest; // đồ trong này sẽ được random
    private Transform transformSpawnPosition;

    private Chest chest;
    private ChestData chestData;

    

    private void Awake()
    {
        // itemsInChest = new List<GameObject>(); // bỏ vì mình tụ sắp xếp giữ dữ liệu trong hòm có gì mà nếu có dòng này thì mất hết dữ liệu
        transformSpawnPosition = gameObject.transform.Find("SpawnPosition");

        chest = gameObject.GetComponent<Chest>();
        chestData = gameObject.GetComponent<ChestData>();
    }

    private void Start()
    {
        chest.OnTriggerChestIsOpended += SpawnItems;

        totalItemsSpawnItemInRandomChest = Random.Range(chestData.minDropItems, chestData.maxDropItems);
    }

    private void SpawnItems()
    {
        bool IsUsedToOpenVar = chest.getIsUsedToOpen();
        if(IsUsedToOpenVar == true) return;

        // Spawn đồ trong rương cố định
        for(int idx = 0; idx < itemsInSetedChest.Count ; idx++) // đây sẽ là những thứ được tự DEV nhét vào
        {
            if(IsUsedToOpenVar == false)
            {
                GameObject itemSpawned = Instantiate(itemsInSetedChest[idx], transformSpawnPosition.position, Quaternion.identity);

                EffectThrowOutItem(itemSpawned);
            }
        }

        // Spawn đồ trong rương ngẫu nhiên
        // C1: Spawn tất cùng một lúc
        // int amountItems = UnityEngine.Random.Range(chestData.minDropItems, chestData.maxDropItems);
        // for(int idx = 0 ; idx < amountItems ; idx++) // đây là sẽ là những thứ được DEV random
        // {
        //     if(IsUsedToOpenVar == false)
        //     {
        //         // Chọn đồ ngẫu nhiên từ hòm để spawn ra
        //         int idxItemRandom = UnityEngine.Random.Range(0, itemsInRandomChest.Count-1);
        //         GameObject itemSpawned = Instantiate(itemsInRandomChest[idxItemRandom], transformSpawnPosition.position, Quaternion.identity);
                
        //         // Hiệu ứng bắn đồ lên
        //         EffectThrowOutItem(itemSpawned);
        //     }
        // }

        // C2: spawn 2-3 món đồ theo nhiều lượt spawn
        FunctionTimer.Create(SpawnItemsCroutineHandler, 0.25f, "SpawnRandomChestItems");
        
    }

    private const int MAX_AMOUNT_ITEMS_SPAWN_FOR_EACH = 3;
    private int totalItemsSpawnItemInRandomChest;
    private int amountSpawnedItemsInRandomChest;
    private int amountSpawnItemsThisTimeInRandomChest;

    private void SpawnItemsCroutineHandler()
    {
        if(amountSpawnedItemsInRandomChest >= totalItemsSpawnItemInRandomChest) return;

        amountSpawnItemsThisTimeInRandomChest = UnityEngine.Random.Range(0, MAX_AMOUNT_ITEMS_SPAWN_FOR_EACH);
          
        if(amountSpawnItemsThisTimeInRandomChest + amountSpawnedItemsInRandomChest > totalItemsSpawnItemInRandomChest)
        {
            amountSpawnItemsThisTimeInRandomChest = totalItemsSpawnItemInRandomChest - amountSpawnedItemsInRandomChest;
        }

        SpawnItemsForEach(amountSpawnItemsThisTimeInRandomChest);

        // sau khi spawn xong thì tính lại số items đã được spawned
        amountSpawnedItemsInRandomChest = calcuateAmountItemsSpawnedInRandomChest(amountSpawnItemsThisTimeInRandomChest, amountSpawnedItemsInRandomChest);

        // tự gọi lại
        FunctionTimer.Create(SpawnItemsCroutineHandler, 0.25f, "SpawnRandomChestItems");
    }

    private void SpawnItemsForEach(int amoutSpawnThisTime)
    {
        for(int i = 0 ; i < amoutSpawnThisTime ; i++)
        {
            // Chọn đồ ngẫu nhiên từ hòm để spawn ra
            int idxItemRandomChest = UnityEngine.Random.Range(0, itemsInRandomChest.Count-1);
            GameObject itemSpawned = Instantiate(itemsInRandomChest[idxItemRandomChest], transformSpawnPosition.position, Quaternion.identity);

            EffectThrowOutItem(itemSpawned);
        }
    }

    private int calcuateAmountItemsSpawnedInRandomChest(int amoutSpawnThisTime, int amountSpawned)
    {
        int newAmountSpawned = amoutSpawnThisTime + amountSpawned;
        return newAmountSpawned;
    }

    private void EffectThrowOutItem(GameObject itemSpawned)
    {
        Rigidbody2D itemRB2D = itemSpawned.GetComponent<Rigidbody2D>();
        if(itemRB2D != null)
        {
            float launchForceX = 2f;
            float launchForceY = 5f;
            itemRB2D.linearVelocity = new Vector3(UnityEngine.Random.Range(-launchForceX, +launchForceX), launchForceY);
        }
    }
}
