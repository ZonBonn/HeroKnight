using System.Collections.Generic;
using UnityEngine;

public class ChestItemsHolder : MonoBehaviour
{
    [SerializeField] private List<GameObject> itemsInChestIsSeted; // đồ trong này sẽ được spawn theo thứ tự ném vào
    [SerializeField] private List<GameObject> itemsInChestIsRandom; // đồ trong này sẽ được random
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
    }

    private void SpawnItems()
    {
        bool IsUsedToOpenVar = chest.getIsUsedToOpen();
        if(IsUsedToOpenVar == true) return;
        for(int idx = 0; idx < itemsInChestIsSeted.Count ; idx++) // đây sẽ là những thứ được tự DEV nhét vào
        {
            if(IsUsedToOpenVar == false)
            {
                GameObject itemSpawned = Instantiate(itemsInChestIsSeted[idx], transformSpawnPosition.position, Quaternion.identity);
                Rigidbody2D itemRB2D = itemSpawned.GetComponent<Rigidbody2D>();
                if(itemRB2D != null)
                {
                    float launchForceX = 2f;
                    float launchForceY = 5f;
                    itemRB2D.linearVelocity = new Vector3(UnityEngine.Random.Range(-launchForceX, +launchForceX), launchForceY);
                }
            }
        }

        int amountItems = UnityEngine.Random.Range(chestData.minDropItems, chestData.maxDropItems);
        for(int idx = 0 ; idx <= amountItems ; idx++) // đây là sẽ là những thứ được DEV random
        {
            if(IsUsedToOpenVar == false)
            {
                int idxItemRandom = UnityEngine.Random.Range(0, itemsInChestIsRandom.Count-1);
                GameObject itemSpawned = Instantiate(itemsInChestIsRandom[idxItemRandom], transformSpawnPosition.position, Quaternion.identity);
                Rigidbody2D itemRB2D = itemSpawned.GetComponent<Rigidbody2D>();
                if(itemRB2D != null)
                {
                    float launchForceX = 2f;
                    float launchForceY = 5f;
                    itemRB2D.linearVelocity = new Vector3(UnityEngine.Random.Range(-launchForceX, +launchForceX), launchForceY);
                }
            }
        }
    }
}
