using System;
using UnityEngine;
using ZonBon.Utils;

public class Grid_S<NodeType>
{
    Transform debugParent = new GameObject("DebugTextContainer").transform;
    // OnGridValueChangedEventArgs: tham số gửi cho subcribers, Action thì thường không có
    public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
    public class OnGridValueChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
    }

    private int width; // == this.width;
    private int height; // == this.height;
    private float cellSize;
    private Vector3 SetRootLocation;

    private NodeType[,] gridArr; // mảng chỉ lưu giữ giá trị không hiển thị được trên màn hình phải thông qua TextMesh => muốn sửa thì làm như nào ?=>
    // muốn sửa thì phải sửa ở cả vị trí gridA[i, j] rồi từ i, j tương ứng ở debugArr[i, j] để sửa
    private TextMesh[,] debugTextArr; // mảng hiện giá trị tương ứng cho ô i, j ở mảng gridArr trên màn hình

    public Grid_S(int width, int height, float cellSize, Vector3 SetRootLocation, Func<Grid_S<NodeType>, int, int, NodeType> createNodeType) // constructor
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.SetRootLocation = SetRootLocation;
        gridArr = new NodeType[width, height];

        // Init gridArr
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                gridArr[i, j] = createNodeType(this, i, j); // cứ gọi tên delegate Func thì nó lại chạy hàm lambda kia thôi 
            }
        }

        bool showDebug = true;
        if (showDebug)
        {
            debugTextArr = new TextMesh[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    debugTextArr[i, j] = UtilsClass.CreateTextInWorld(gridArr[i, j]?.ToString(), GetWorldPosition(i, j) + new Vector3(cellSize, cellSize) * 0.5f, 40, cellSize / 10f, debugParent);
                    Debug.DrawLine(GetWorldPosition(i, j), GetWorldPosition(i, j + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(i, j), GetWorldPosition(i + 1, j), Color.white, 100f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
            OnGridValueChanged += (object sender, OnGridValueChangedEventArgs eventArgs) =>
            {
                debugTextArr[eventArgs.x, eventArgs.y].text = gridArr[eventArgs.x, eventArgs.y]?.ToString();
            };
        }
    }

    public NodeType GetNodeType(int i, int j)
    {
        if (i >= 0 && j >= 0 && i < width && j < height)
        {
            return gridArr[i, j];
        }
        else
        {
            return default(NodeType);
        }
    }

    public NodeType GetNodeType(Vector3 ScreenPos)
    {
        Vector3 WorldPos = UtilsClass.getMouseWorldPosition();
        int i, j;
        worldPosToIJPos(WorldPos, out i, out j);
        if (i >= 0 && j >= 0 && i < width && j < height)
        {
            return gridArr[i, j];
        }
        else
        {
            return default(NodeType);
        }
    }

    public void SetNodeType(int i, int j, NodeType value) // set value khi có một tọa độ hợp lệ là i, j
    {
        if (i >= 0 && i < width && j >= 0 && j < height)
        {
            gridArr[i, j] = value;
            debugTextArr[i, j].text = gridArr[i, j].ToString();
        }
        if (OnGridValueChanged != null)
        {
            //OnGridValueChanged(this, new OnGridValueChangedEventArgs {x = i, y = j}) tương đương
            // <=> OnGridValueChanged.Invoke(this, new OnGridValueChangedEventArgs {x = i, y = j})
            OnGridValueChanged.Invoke(this, new OnGridValueChangedEventArgs { x = i, y = j });
        }
    }

    public void SetNodeType(Vector3 worldPosition, NodeType value) // set value khi có một tọa độ hợp lệ trong world
    {
        int i, j;
        worldPosToIJPos(worldPosition, out i, out j);
        SetNodeType(i, j, value);
    }

    public void worldPosToIJPos(Vector3 worldPosition, out int i, out int j)
    {
        i = Mathf.FloorToInt((worldPosition.x - SetRootLocation.x) / cellSize);
        j = Mathf.FloorToInt((worldPosition.y - SetRootLocation.y) / cellSize);
    }  

    private Vector3 GetWorldPosition(int i, int j) // vị trí đặt của Text Mesh
    {
        return new Vector3(i, j) * cellSize + SetRootLocation;
    }


    public void OnTriggerNodeTypeObjectChanged(int i, int j)
    {
        if (OnGridValueChanged != null) OnGridValueChanged.Invoke(this, new OnGridValueChangedEventArgs { x = i, y = j });
    }

    public void SetDebugArr(int i, int j)
    {
        if (debugTextArr != null)
        {
            debugTextArr[i, j].text = "X";
            debugTextArr[i, j].color = Color.red;
        }
    }

    public void SetNormallyDebugArr(int i, int j)
    {
        debugTextArr[i, j].text = gridArr[i, j].ToString();
        debugTextArr[i, j].color = Color.white;
    }
    public string GetDebugArr(int i, int j)
    {
        if (i >= 0 && i < width && j >= 0 && j < height)
        {
            return debugTextArr[i, j].text;
        }
        return null;
    }

    public bool isInGrid(int i, int j)
    {
        if (i >= 0 && i < width && j >= 0 && j < height)
        {
            return true;
        }
        return false;
    }

    public Vector3 getRootLocation()
    {
        return SetRootLocation;
    }

    public float getCellSize()
    {
        return cellSize;
    }

    public int getWidth()
    {
        return this.width;
    }

    public int getHeight()
    {
        return this.height;
    }

    public NodeType getNodeTypeByWorldPosition(Vector3 WorldPosition)
    {
        worldPosToIJPos(WorldPosition, out int i, out int j);
        return gridArr[i, j];
    }

    public NodeType getNodeTypeByGridPosition(int i, int j)
    {
        return gridArr[i, j];
    }
}