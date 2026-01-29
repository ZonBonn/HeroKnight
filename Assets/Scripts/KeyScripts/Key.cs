using UnityEngine;

public class Key : MonoBehaviour
{
    public enum KeyType
    {
        Wooden,
        Golden,
        Iron,
        Silver
    }

    [SerializeField] private KeyType keyType;

    private bool CanPickUpKey;

    private void Awake()
    {
        CanPickUpKey = false;

        FunctionTimer.Create(SetCanPickUpTrue, 1f);
    }

    public KeyType GetKeyType()
    {
        return keyType;
    }

    // check va chạm với người chơi
    // private void OnTriggerEnter2D(Collider2D collider2D)
    // {
    //     if(collider2D.CompareTag("Player") == true && CanPickUp)
    //     {
            
    //     }
    // }

    // +1 kinh nghiệm: wtf ??? code kém ?? chỉ cần set khi nó được sinh ra thì hàm Awake{} sẽ chạy timer đếm ngược có thể pick ngay trong chính Key cần gì phải tham chiếu kiểu sang tận các file khác lằng nhàng như này ?????
    private void SetCanPickUpTrue() 
    {
        CanPickUpKey = true;
    }

    public bool getCanPickUpKey()
    {
        return CanPickUpKey;
    }
}
