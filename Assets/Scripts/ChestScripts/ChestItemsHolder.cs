using System.Collections.Generic;
using UnityEngine;

public class ChestItemsHolder : MonoBehaviour
{
    [SerializeField] private List<GameObject> itemsInSetedChest; // đồ trong này sẽ được spawn theo thứ tự ném vào
    [SerializeField] private List<GameObject> itemsInRandomChest; // đồ trong này sẽ được random
    private Transform transformSpawnPosition;

    private Chest chest;
    private ChestData chestData;

    private const int MAX_AMOUNT_ITEMS_SPAWN_FOR_EACH = 4; // thực ra là = 3 vì trong random phần max nó lấy cận dưới   
    private int totalItemsSpawnInRandomChest;
    private int amountSpawnedItemsInRandomChest;
    private int amountSpawnItemsThisTimeInRandomChest;

    private bool spawnedAllItems; // new @@@
    

    private void Awake()
    {
        // itemsInChest = new List<GameObject>(); // bỏ vì mình tụ sắp xếp giữ dữ liệu trong hòm có gì mà nếu có dòng này thì mất hết dữ liệu
        transformSpawnPosition = gameObject.transform.Find("SpawnPosition");
        if (transformSpawnPosition == null) Debug.LogError("SpawnPosition not found!"); // for safe

        chest = gameObject.GetComponent<Chest>();
        chestData = gameObject.GetComponent<ChestData>();
    }

    private void Start()
    {
        chest.OnTriggerChestIsOpended += SpawnItems;

        // init var
        totalItemsSpawnInRandomChest = Random.Range(chestData.minDropItems, chestData.maxDropItems);
        amountSpawnedItemsInRandomChest = 0;

        spawnedAllItems = false;
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

        // C2: spawn 1-3 món đồ theo nhiều lượt spawn
        if (itemsInRandomChest.Count > 0) // nếu random chest không có item thì rất dễ Out of range hoặc bị treo bởi dòng code dưới này
        {
            FunctionTimer.Create(SpawnItemsCroutineHandler, 0.5f);
        }
    }


    private void SpawnItemsCroutineHandler()
    {
        if(amountSpawnedItemsInRandomChest >= totalItemsSpawnInRandomChest)
        {
            // chả lẽ gọi thêm một cái FunctionTimer ở đây chờ 1s mới cho đóng cái hòm lần đầu :D
            FunctionTimer.Create(setSpawnedAllItemsTrue, 0.5f);
            return;
        }

        amountSpawnItemsThisTimeInRandomChest = UnityEngine.Random.Range(1, MAX_AMOUNT_ITEMS_SPAWN_FOR_EACH);
          
        if(amountSpawnItemsThisTimeInRandomChest + amountSpawnedItemsInRandomChest > totalItemsSpawnInRandomChest)
        {
            amountSpawnItemsThisTimeInRandomChest = totalItemsSpawnInRandomChest - amountSpawnedItemsInRandomChest;
        }

        SpawnItemsForEach(amountSpawnItemsThisTimeInRandomChest);

        // sau khi spawn xong thì tính lại số items đã được spawned
        amountSpawnedItemsInRandomChest = calculateAmountItemsSpawnedInRandomChest(amountSpawnItemsThisTimeInRandomChest, amountSpawnedItemsInRandomChest);

        // tự gọi lại: KHÁ NGUY HIỂM NẾU KHÔNG KIÊM SOÁT CẨN THẬN CÓ NGUY CƠ ĐỨNG TREO GAME (đệ quy)
        FunctionTimer.Create(SpawnItemsCroutineHandler, 0.5f);
    }

    private void SpawnItemsForEach(int amoutSpawnThisTime)
    {
        for(int i = 0 ; i < amoutSpawnThisTime ; i++)
        {
            // Chọn đồ ngẫu nhiên từ hòm để spawn ra
            int idxItemRandomChest = UnityEngine.Random.Range(0, itemsInRandomChest.Count); // [min, max)
            GameObject itemSpawned = Instantiate(itemsInRandomChest[idxItemRandomChest], transformSpawnPosition.position, Quaternion.identity);

            EffectThrowOutItem(itemSpawned);
        }
    }

    private int calculateAmountItemsSpawnedInRandomChest(int amoutSpawnThisTime, int amountSpawned)
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

    private void setSpawnedAllItemsTrue()
    {
        spawnedAllItems = true;
    }

    public bool getSpawnedAllItems()
    {
        return spawnedAllItems;
    }
}
