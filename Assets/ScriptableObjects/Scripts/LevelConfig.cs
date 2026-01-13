using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "LevelConfigureScriptableObject/LevelConfig")]
public class LevelConfig : ScriptableObject
{
    public int level;
    
    public int amountSpawnedWoodenChest;
    public int amountSpawnedIronChest;
    public int amountSpawnedSilverChest;
    public int amountSpawnedGoldenChest;

    public int maxWoodenKeyAllowed;
    public int maxIronKeyAllowed;
    public int maxSilverKeyAllowed;
    public int maxGoldenKeyAllowed;
}
