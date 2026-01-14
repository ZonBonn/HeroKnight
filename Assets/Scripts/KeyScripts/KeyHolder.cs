using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class KeyHolder : MonoBehaviour
{
    private List<Key.KeyType> keyList;

    public Action OnTriggerPlayerOpenTheChest;
    
    private bool CanPickUpKey;
    
    private void Awake()
    {
        keyList = new List<Key.KeyType>();

        CanPickUpKey = false;
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
        // Check va chạm với chìa khóa
        Key key = collider.gameObject.GetComponent<Key>();
        if(key != null && key.getCanPickUpKey() == true) // nếu cái va chạm chính là chìa khóa thì:
        {
            Debug.Log("Add key" + key.GetKeyType());
            AddKey(key.GetKeyType());
            Destroy(key.gameObject); // xóa Object chìa khóa đi vì player nhặt rồi
        }

        // Check va chạm và có thể mở được rương hay không ?
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
                        OnTriggerPlayerOpenTheChest?.Invoke();

                        ChestAnimation chestAnimation = collider.GetComponent<ChestAnimation>();
                        chestAnimation.SetCanKeepOpenForFirstTime(true);

                        RemoveKey(IsPlayerHoldingThisKey);
                    }
                }
            }
        }
    }

    public void SetCanPickUpKeyTrue()
    {
        CanPickUpKey = true;
    }
}
