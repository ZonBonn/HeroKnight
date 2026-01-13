using UnityEngine;

public class KeyManager : MonoBehaviour
{
    [SerializeField] private LevelConfig levelConfig;
    private int currentSpawnedWoodenKey;
    private int currentSpawnedIronKey;
    private int currentSpawnedSilverKey;
    private int currentSpawnedGoldenKey;

    private void Awake()
    {
        
    }

    private void Start()
    {
        
    }

    private int getAmountSpawnedKeyByKeyType(Key.KeyType keyType)
    {
        if(keyType == Key.KeyType.Wooden)
        {
            return currentSpawnedWoodenKey;
        }
        else if(keyType == Key.KeyType.Iron)
        {
         return currentSpawnedIronKey;   
        }
        else if(keyType == Key.KeyType.Silver)
        {
            return currentSpawnedSilverKey;
        }
        else if(keyType == Key.KeyType.Golden)
        {
           return currentSpawnedGoldenKey; 
        }
        else
        {
            Debug.Log("chưa có loại keyType này thêm else if nữa đi, return => 0");
            return 0;
        }
    }

    public bool IsCanSpawnKey(Key.KeyType keyType)
    {
        // if(getAmountSpawnedKeyByKeyType(keyType) < levelConfig.)
        return true;
    }
}
