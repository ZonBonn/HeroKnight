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

    // +1 kinh nghiệm: chỉ cần set khi nó được sinh ra thì hàm Awake{} sẽ chạy timer đếm ngược có thể pick ngay trong chính Key
    private void SetCanPickUpTrue() 
    {
        CanPickUpKey = true;
    }

    public bool getCanPickUpKey()
    {
        return CanPickUpKey;
    }
}
