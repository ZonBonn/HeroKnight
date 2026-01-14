using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;

public class KeyManager : MonoBehaviour
{
    [SerializeField] private LevelConfig levelConfig;

    Dictionary<Key.KeyType, int> spawnedKey;
    Dictionary<Key.KeyType, int> limitAmountKeyType;

    private void Awake()
    {
        spawnedKey = new Dictionary<Key.KeyType, int>();
        limitAmountKeyType = new Dictionary<Key.KeyType, int>();

        // set up limitAmountKeyType
       for(int i = 0 ; i < levelConfig.limitAmountKeyType.Count ; i++)
        {
            limitAmountKeyType.Add(levelConfig.limitAmountKeyType[i].keyType, levelConfig.limitAmountKeyType[i].limitAmountKeyType);
        }
    }

    private void Start()
    {
        // EnemyItemsHolder.OnDropKey += IncreaseAmountKey(); // gửi cả giá trị nữa ---> continue your work in here <---
    }

    public int getAmountSpawnedKeyByKeyType(Key.KeyType keyType)
    {
        if(spawnedKey.TryGetValue(keyType, out int amountSpawnedThisKeyType))
        {
            return amountSpawnedThisKeyType;
        }
        return 0;
    }

    public void IncreaseAmountKey(Key.KeyType keyType)
    {
        if (!spawnedKey.ContainsKey(keyType))
        {
            spawnedKey[keyType] = 0;
        }
        spawnedKey[keyType]++;
    }

    public bool IsCanSpawnKey(Key.KeyType keyType)
    {
        int maxAmountOfThisKeyType = limitAmountKeyType[keyType];
        return getAmountSpawnedKeyByKeyType(keyType) < maxAmountOfThisKeyType;
    }
}
