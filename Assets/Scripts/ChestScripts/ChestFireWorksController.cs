using UnityEngine;

public class ChestFireWorksController : MonoBehaviour
{
    public enum ChestHaveFireWorks { Yes, No }
    public ChestHaveFireWorks chestHaveFireWorks;
    public FireWorks fireWorks;
    private Chest chest;

    private void Awake()
    {
        chest = gameObject.GetComponent<Chest>();
    }
    
    private void Start()
    {
        if(chestHaveFireWorks == ChestHaveFireWorks.No)
        {
            this.enabled = false;
        }

        chest.OnTriggerChestIsOpended += SetFireWorksOn;
        chest.OnTriggerPlayerNearChest += SetFireWorksOn;
        chest.OnTriggerPlayerFarChest += SetFireWorksOff;
    }

    
    public void SetFireWorksOn() // bật pháo hoa <=> người chơi gần và đã từng hoặc đang được mở (khởi chạy khi mở rương && mở rương lần đầu)
    { 
        if(fireWorks.gameObject.activeInHierarchy == false && chest.getIsPlayerNear() == true) fireWorks.gameObject.SetActive(true); 
    }

    public void SetFireWorksOff() // tắt pháo hoa <=> (khởi chạy 1 lần khi người chơi out khỏi collider check isPlayerNear)
    { 
        if(fireWorks.gameObject.activeInHierarchy == true) fireWorks.gameObject.SetActive(false); 
    }

}
