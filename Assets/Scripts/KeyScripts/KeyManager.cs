using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;

public class KeyManager : MonoBehaviour
{
    static Dictionary<Key.KeyType, int> spawnedKey; // lưu trữ dữ liệu key đã và đang sẽ được spawn
    static Dictionary<Key.KeyType, int> limitAmountKeyType; // lưu trữ dữ liệu từ LevelConfig

    public static bool IsCanSpawnKey(Key.KeyType keyType) // hàm drop theo số lượng
    {
        if(!limitAmountKeyType.TryGetValue(keyType, out int maxAmountOfThisKeyType))
            return false;

        return getAmountSpawnedKeyByKeyType(keyType) < maxAmountOfThisKeyType;
    }

    public static int getAmountSpawnedKeyByKeyType(Key.KeyType keyType)
    {
        if(spawnedKey.TryGetValue(keyType, out int amountSpawnedThisKeyType))
        {
            return amountSpawnedThisKeyType;
        }
        return 0;
    }



    [SerializeField] private LevelConfig levelConfig;

    private void Awake()
    {
        spawnedKey = new Dictionary<Key.KeyType, int>();
        limitAmountKeyType = new Dictionary<Key.KeyType, int>();

        // reset for each scene
        spawnedKey.Clear();
        limitAmountKeyType.Clear();

        // set up limitAmountKeyType
       for(int i = 0 ; i < levelConfig.limitAmountKeyType.Count ; i++)
        {
            limitAmountKeyType.Add(levelConfig.limitAmountKeyType[i].keyType, levelConfig.limitAmountKeyType[i].limitAmountKeyType);
        }
    }

    private void Start()
    {
        EnemyItemsHolder.OnTriggerDropKey += Wrapper; // gửi cả giá trị nữa ---> continue your work in here <---
    }

    private void Update()
    {
        ChoseToWatch();
    }

    public void IncreaseAmountKey(Key.KeyType keyType)
    {
        if (!spawnedKey.ContainsKey(keyType))
        {
            spawnedKey[keyType] = 0;
        }
        spawnedKey[keyType]++;
    }

    public void Wrapper(object sender, EnemyItemsHolder.DropKeyEventArgs dropKeyEventArgs) // tăng cái key được spawn ra
    {
        Key.KeyType keyType = dropKeyEventArgs.keyType;
        IncreaseAmountKey(keyType);
    }

    public void OnDestroy()
    {
        EnemyItemsHolder.OnTriggerDropKey -= Wrapper;
    }

    public void ChoseToWatch()
    {
        if (Input.GetKey(KeyCode.C) && Input.GetKey(KeyCode.W))
        {
            CheckAmountSpawnedKeyAndMaximumKey(Key.KeyType.Wooden);
        }
        else if (Input.GetKey(KeyCode.C) && Input.GetKey(KeyCode.I))
        {
            CheckAmountSpawnedKeyAndMaximumKey(Key.KeyType.Iron);
        }
        else if (Input.GetKey(KeyCode.C) && Input.GetKey(KeyCode.S))
        {
            CheckAmountSpawnedKeyAndMaximumKey(Key.KeyType.Silver);
        }
        else if (Input.GetKey(KeyCode.C) && Input.GetKey(KeyCode.G))
        {
            CheckAmountSpawnedKeyAndMaximumKey(Key.KeyType.Golden);
        }
    }

    private void CheckAmountSpawnedKeyAndMaximumKey(Key.KeyType keyType)
    {
        if(spawnedKey.ContainsKey(keyType) == false)
        {
            Debug.Log("spawnedKey" + 0 + " limitAmountKeyType:" + limitAmountKeyType[keyType]);
        }
        else
            Debug.Log("spawnedKey" + spawnedKey[keyType] + " limitAmountKeyType:" + limitAmountKeyType[keyType]);
    }
}
