using System.Collections.Generic;
using UnityEngine;

public class DropRandomItems : MonoBehaviour
{
    [SerializeField] List<ItemsData> randomItemsDropList;

    private HealthHandler healthHandler;
    private HealthSystem healthSystem;

    
    [System.Serializable] public struct ItemsData
    {
        public float rateDrop;
        public GameObject itemGO;
    }


    private void Awake()
    {
        healthHandler = gameObject.GetComponent<HealthHandler>();
        healthSystem = healthHandler.GetHealthSystem();
    }

    private void Start()
    {
        healthSystem.OnTriggerHealthBarAsZero += StartSpawnRandomItems;
    }

    private void Update()
    {
        
    }

    private void StartSpawnRandomItems()
    {
        for(int i = 0 ; i < randomItemsDropList.Count ; i++)
        {
            if (CanDropByRate(randomItemsDropList[i].rateDrop))
            {
                GameObject item = Instantiate(randomItemsDropList[i].itemGO, gameObject.transform.position, Quaternion.identity);
                EffectDropItemsVertical(item);
            }
        }
    }

    private bool CanDropByRate(float rateDropItems)
    {
        float randomRate = Random.Range(0f, 1f);
        // Debug.Log("randomRate:" + randomRate + "  ?  " + "rateDropItems:"+rateDropItems);
        if(randomRate <= rateDropItems)
        {
            return true;
        }
        return false;
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
}
