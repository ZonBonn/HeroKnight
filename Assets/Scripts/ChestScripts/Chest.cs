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

    private bool isPlayerNear; // cái này thì người chơi cứ đến gần là mở xa thì đóng // nhưng đồ chỉ spawn 1 lần
    private bool IsOpended;
    private bool isUsedToOpen; // cái này dùng để chỉ spawn đồ  1 lần

    private void Start()
    {
        isPlayerNear = false; isUsedToOpen = false;
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
            IsOpended = true;
        }    
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") == true)
        {
            isPlayerNear = false;
            IsOpended = false;
        } 
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
}
