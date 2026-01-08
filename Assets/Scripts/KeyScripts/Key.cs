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

    public KeyType GetKeyType()
    {
        return keyType;
    }
}
