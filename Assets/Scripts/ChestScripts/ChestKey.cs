using UnityEngine;

public class ChestKey : MonoBehaviour
{
    [SerializeField] private Key.KeyType keyType; // player cần loại chìa khóa này để mở cái rương này

    public Key.KeyType GetKeyType()
    {
        return keyType;
    }
}
