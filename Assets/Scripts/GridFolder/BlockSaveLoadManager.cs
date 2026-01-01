using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.Android.Gradle;

[System.Serializable]
public class BlockData // wrapper vì Utility.Json thực hiện trên object/class chứ không hoạt đông trên các DS độc lập đứng một mình
{
    public List<Vector2Int> BlockNodesList; // chỉ dùng để container dữ liệu tạm thời và bên trong class BlockData để dùng được ToJson và FromJson
    public BlockData(List<Vector2Int> BlockNodesListOutSide)
    {
        this.BlockNodesList = BlockNodesListOutSide;
    }
}

public class BlockSaveLoadManager : MonoBehaviour
{
    public GridMap refGridMap;
    private PathFinding pathFinding;
    List<Vector2Int> BlockNodeGridPositionList = new List<Vector2Int>(); // chỉ dùng để container dữ liệu lâu dài

    private string saveFilePath;

    public string nameLevel;

    private void Awake()
    { 
        string fileName = $"blockedNodesHeroKnight{nameLevel}.json";
        // Debug.Log(fileName);
        
        string persistentPath = Path.Combine(Application.persistentDataPath, fileName); // đường dẫn tới file "blockNodes.json" -> ngoài project folder
        string sourcePath = Path.Combine(Application.streamingAssetsPath, fileName); // đường dẫn tới file "blockNodes.json" ở trong Assets -> StreamingAssets

        if (!File.Exists(persistentPath)) // nếu chưa tồn tại
        {
            File.Copy(sourcePath, persistentPath); // thì copy file từ 
        }

    }

    private void Start()
    {
        saveFilePath = Application.persistentDataPath + $"/blockedNodesHeroKnight{nameLevel}.json";
        // Debug.Log(saveFilePath);
        pathFinding = refGridMap.pathFinding;
        LoadData();
        AddlyBlockNode();
    }

    private void LoadData()
    {
        if (File.Exists(saveFilePath)) // file tồn tại
        {
            string json = File.ReadAllText(saveFilePath); // đọc tất file json thành chuỗi
            BlockData data = JsonUtility.FromJson<BlockData>(json); // tự dộng gán các field trong class BlockData // chuyển dữ liệu string json thành dữ liệu kiểu BlockData
            BlockNodeGridPositionList = data.BlockNodesList; // chuyển xong rồi thì lấy dữ liệu ra từ data của kiểu BlockData thôi
        }
        else
        {
            SaveData();
        }
    }

    private void SaveData()
    {
        string json = JsonUtility.ToJson(new BlockData(BlockNodeGridPositionList)); // chuyển dữ liệu của List<Vector2> -> string json
        File.WriteAllText(saveFilePath, json); // ghi đè dữ liệu kiểu string json vào file tại đường dẫn saveFilePath (chính là file .json)
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
