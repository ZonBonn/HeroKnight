using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.Android.Gradle;

[System.Serializable]
public class BlockData // wrapper vì Utility.Json thực hiện trên object/class chứ không hoạt đông trên các DS độc lập đứng một mình
{
    public List<Vector2Int> BlockNodesList;
    public BlockData(List<Vector2Int> BlockNodesListOutSide)
    {
        this.BlockNodesList = BlockNodesListOutSide;
    }
}

public class BlockSaveLoadManager : MonoBehaviour
{
    public GridMap refGridMap;
    private PathFinding pathFinding;
    List<Vector2Int> BlockNodeGridPositionList = new List<Vector2Int>();

    private string saveFilePath;

    public string nameLevel;

    private void Awake()
    { 
        string persistentPath = Path.Combine(Application.persistentDataPath, "blockedNodesHeroKnight.json"); // đường dẫn tới file "blockNodes.json" -> ngoài project folder
        string sourcePath = Path.Combine(Application.streamingAssetsPath, "blockedNodesHeroKnight.json"); // đường dẫn tới file "blockNodes.json" ở trong Assets -> StreamingAssets

        if (!File.Exists(persistentPath)) // nếu chưa tồn tại
        {
            File.Copy(sourcePath, persistentPath); // thì copy file từ 
        }

    }

    private void Start()
    {
        saveFilePath = Application.persistentDataPath + "/blockedNodesHeroKnight.json";
        // Debug.Log(saveFilePath);
        pathFinding = refGridMap.pathFinding;
        LoadData();
        AddlyBlockNode();
    }

    private void LoadData()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            BlockData data = JsonUtility.FromJson<BlockData>(json); // tự dộng gán các field trong class BlockData
            BlockNodeGridPositionList = data.BlockNodesList;
        }
        else
        {
            SaveData();
        }
    }

    private void SaveData()
    {
        string json = JsonUtility.ToJson(new BlockData(BlockNodeGridPositionList));
        File.WriteAllText(saveFilePath, json);
    }

    private void AddlyBlockNode()
    {
        foreach (Vector2Int BlockNode in BlockNodeGridPositionList)
        {
            // Debug.Log("Add block node at i:" + BlockNode.x + " ; j:" + BlockNode.y);
            this.pathFinding.setBlockNodeByGridPosition(BlockNode.x, BlockNode.y);
        }
    }

    public void addNodeToBlockNodeGridPositionList(int i, int j)
    {
        Vector2Int tmp = new Vector2Int(i, j);
        BlockNodeGridPositionList.Add(tmp);
        SaveData();
    }

    public void removeNodeFromBlockNodeGridPositionList(int i, int j)
    {
        for (int idx = 0; idx < BlockNodeGridPositionList.Count; idx++)
        {
            if (BlockNodeGridPositionList[idx].x == i && BlockNodeGridPositionList[idx].y == j)
            {
                // Debug.Log("Remove Block Node at i:" + i + " ; j:" + j);
                BlockNodeGridPositionList.RemoveAt(idx);
                SaveData();
                return;
            }
        }
    }
}
