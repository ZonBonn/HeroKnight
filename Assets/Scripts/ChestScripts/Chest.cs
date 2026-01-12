using System;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public enum ChestType
    {
        Golden,
        Silver,
        Wooden,
        Iron
    } 

    [SerializeField] public ChestType chestType;
    private ChestKey chestKey;

    private bool isPlayerNear; // cái này thì người chơi cứ đến gần là mở xa thì đóng // nhưng đồ chỉ spawn 1 lần
    private bool IsOpended;
    private bool isUsedToOpen; // cái này dùng để chỉ spawn đồ  1 lần

    private bool isFristTimeOpen;

    public Action OnTriggerChestIsOpended;

    private ChestItemsHolder chestItemsHolder;

    private void Awake()
    {
        chestKey = gameObject.GetComponent<ChestKey>();
    }
    
    private void Start()
    {
        isPlayerNear = false; isUsedToOpen = false; isFristTimeOpen = false;
    }

    private void Update()
    {
        
    }

    // ============= CHECK FUNCTION ==================
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") == true)
        {
            isPlayerNear = true;
            // check nếu có key ở đây thì mới mở được (nhưng tạm thời test thì chỉ cần người chơi gần là mở rồi)
            // IsOpended = true;
        }    
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") == true)
        {
            isPlayerNear = false;
            // IsOpended = false;
        } 
    }

    public bool TryOpen(Key.KeyType keyType)
    {
        Key.KeyType keyNeedToOpenThisChest = chestKey.GetKeyType();
        if(keyType != keyNeedToOpenThisChest)
        {
            return false;
        }
        Open();
        return true;
    }

    private void Open()
    {
        OnTriggerChestIsOpended?.Invoke();
        IsOpended = true;
        isUsedToOpen = true; // cái này sẽ được set true không phải ở đây mà là do Delegate của cái thằng ChestItemsHolder: OnTriggerSpawnedAllItems
    }

    // ============== SUPPORT FUNCTION ================
    public ChestType GetChestType()
    {
        return chestType;
    }

    public bool getIsPlayerNear()
    {
        return isPlayerNear;
    }

    public bool getIsUsedToOpen()
    {
        return isUsedToOpen;
    }

    public bool getIsOpended()
    {
        return IsOpended;
    }

    public void setIsFristTimeOpen(bool canOpen)
    {
        isFristTimeOpen = canOpen;
    }

    public bool getIsFristTimeOpen()
    {
        return isFristTimeOpen;
    }

    public void setIsUsedToOpenTrue()
    {
        
    }
    // ==================================================
}
