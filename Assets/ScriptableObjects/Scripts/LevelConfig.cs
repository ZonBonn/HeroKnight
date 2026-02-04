using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "LevelConfigureScriptableObject/LevelConfig")]
public class LevelConfig : ScriptableObject
{
    public int level;

    [SerializeField] public List<KeyLimit> limitAmountKeyType; // giới hạn số lượng key mỗi màn

    [System.Serializable] public struct KeyLimit
    {
        public Key.KeyType keyType; // loại key
        public int limitAmountKeyType; // số lượng giới hạn của key đó
    }
}
