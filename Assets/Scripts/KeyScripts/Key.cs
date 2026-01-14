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

    public void SetCanPickUpTrue()
    {
        CanPickUpKey = true;
    }

    public bool getCanPickUpKey()
    {
        return CanPickUpKey;
    }
}
