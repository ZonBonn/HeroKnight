using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KeyHolder : MonoBehaviour
{
    private List<Key.KeyType> keyList;

    private void Awake()
    {
        keyList = new List<Key.KeyType>();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    private void AddKey(Key.KeyType keyType)
    {
        keyList.Add(keyType);
    }

    private void RemoveKey(Key.KeyType keyType)
    {
        keyList.Remove(keyType);
    }

    private bool ContainsKey(Key.KeyType keyType)
    {
        return keyList.Contains(keyType);    
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Key key = collider.gameObject.GetComponent<Key>();
        if(key != null) // nếu cái va chạm chính là chìa khóa thì:
        {
            Debug.Log("Add key" + key.GetKeyType());
            AddKey(key.GetKeyType());
            Destroy(key.gameObject); // xóa Object chìa khóa đi vì player nhặt rồi
        }

        ChestKey chestKey = collider.GetComponent<ChestKey>();
        if(chestKey != null)
        {
            Key.KeyType IsPlayerHoldingThisKey = chestKey.GetKeyType();
            if(ContainsKey(IsPlayerHoldingThisKey) == true)
            {
                Chest chest = collider.GetComponent<Chest>();
                if(chest != null)
                {
                    bool IsCanOpen = chest.TryOpen(IsPlayerHoldingThisKey);
                    if(IsCanOpen == true)
                    {
                        RemoveKey(IsPlayerHoldingThisKey);
                    }
                }
            }
        }
    }
}
