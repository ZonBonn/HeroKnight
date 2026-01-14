using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "LevelConfigureScriptableObject/LevelConfig")]
public class LevelConfig : ScriptableObject
{
    public int level;

    [SerializeField] public List<KeyLimit> limitAmountKeyType;

    [System.Serializable] public struct KeyLimit
    {
        public Key.KeyType keyType; 
        public int limitAmountKeyType;
    }
}
