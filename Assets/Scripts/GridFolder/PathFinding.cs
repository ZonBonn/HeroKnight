using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Lumin;
using ZonBon.Utils;


public class PathFinding
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    public static PathFinding Instance { get; private set; }

    public int width;
    public int height;
    public float cellSize;
    public Vector3 SetRootLocation;

    private Grid_S<PathNode_S> grid; // tìm đường trên lưới này

    MinHeap<PathNode_S> openMinHeapList;
    HashSet<PathNode_S> openHashSet;
    HashSet<PathNode_S> closeHashSet;

    public PathFinding(int width, int height, float cellSize, Vector3 SetRootLocation)
    {
        Instance = this;
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.SetRootLocation = SetRootLocation;
        grid = new Grid_S<PathNode_S>(width, height, cellSize, SetRootLocation, (Grid_S<PathNode_S> grid, int i, int j) => new PathNode_S(grid, i, j));
        
        // init neighbours for each PathNode_S
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                getNeighbourNodeList(grid.GetNodeType(i, j));
            }
        }
    }

    public List<Vector3> PathOnVector(Vector3 startWorldPosition, Vector3 endWorldPosition, out List<PathNode_S> PathOnNode/*, MonsterType monsterType*/) //hàm chuyển vị trí đường đi i,j -> đường đi theo world position
    {
        grid.worldPosToIJPos(startWorldPosition, out int startI, out int startJ);
        grid.worldPosToIJPos(endWorldPosition, out int endI, out int endJ);

        // MỤC NÀY ĐỂ PHÙ HỢP VỚI TỰA GAME PLATFORM TÙY CHỈNH SAO CHO MỤC ENDI VÀ ENDJ NÓ Ở Ô STANDABLE CHỨ KHÔNG PHẢI TRÊN KHÔNG THÌ SẼ KHÔNG NULL
        if(TryGetGroundNodeBelow(endI, endJ, out PathNode_S groundNode) == false)
        {
            PathOnNode = null;
            return null;
        }

        int groundNodeEndI = groundNode.i;
        int groundNodeEndJ = groundNode.j;

        PathOnNode = Path(startI, startJ, groundNodeEndI, groundNodeEndJ);
        List<Vector3> PathOnVector = new List<Vector3>();

        if (PathOnNode != null)
        {
            foreach (PathNode_S PathNode in PathOnNode)
            {
                PathOnVector.Add(new Vector3(PathNode.i, PathNode.j) * grid.getCellSize() + (Vector3.one * grid.getCellSize() *0.5f) + grid.getRootLocation());
            } 
            return PathOnVector;
        }
        else
        {
            // Debug.Log("There is no path on Vector for " + monsterType);
            return null;
        }
    }

    public List<PathNode_S> Path(int startI, int startJ, int endI, int endJ)
    {
        PathNode_S startNode = grid.GetNodeType(startI, startJ);
        PathNode_S endNode = grid.GetNodeType(endI, endJ);

        openMinHeapList = new MinHeap<PathNode_S>(); openMinHeapList.addNewNode(startNode);
        openHashSet = new HashSet<PathNode_S> {startNode};
        closeHashSet = new HashSet<PathNode_S>();

        // Init Grid Object Value (khởi tạo các giá trị bên trong từng Node)
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                PathNode_S tmpNode = grid.GetNodeType(i, j);
                tmpNode.gCost = int.MaxValue;
                tmpNode.calculateFCost();
                tmpNode.cameFromThisNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = calculateDistanceBtwTwoNode(startNode, endNode);
        startNode.calculateFCost();

        while (openMinHeapList.getCount() > 0)
        {
            PathNode_S currentNode = openMinHeapList.getLowestFCostNode();
            if (currentNode == endNode)
            {
                // đã tới đích
                return calculatePath(endNode);
            }
            else
            {
                openMinHeapList.removeNodeAtFirst(); openHashSet.Remove(currentNode);
                closeHashSet.Add(currentNode);
                foreach (PathNode_S NeighbourNode in currentNode.NeighboursList) // duyệt qua các neighbourNode của currentNode
                {
                    if (NeighbourNode.isWalkable == false)
                    {
                        closeHashSet.Add(NeighbourNode);
                        continue;
                    }
                    if (closeHashSet.Contains(NeighbourNode)) continue;// nếu đã bị từng là currentNode (nếu từng là currentNode thì có nghĩa là đã được tính chi phí tối ưu)

                    // MỤC NÀY ĐỂ PHÙ HỢP VỚI TỰA GAME PLATFORM
                    if(IsStandable(NeighbourNode) == false) continue;

                    // tentativeGCost == chi phí đi từ ô start -> currentNode + tới ô neighbourNode <==> startNode -> neighbourNode
                    int tentativeGCost = currentNode.gCost + calculateDistanceBtwTwoNode(currentNode, NeighbourNode);
                    if (tentativeGCost < NeighbourNode.gCost) // nếu đường đi từ ô startNode -> neighbourNode tốt hơn đường trước đó
                    {
                        NeighbourNode.gCost = tentativeGCost; // set lại từ startNode -> neighbourNode
                        NeighbourNode.hCost = calculateDistanceBtwTwoNode(NeighbourNode, endNode); // tính lại từ neighbourNode -> endNode (chắc khởi tạo vì H Cost luôn như vậy mà )
                        NeighbourNode.calculateFCost(); // tính lại neighbourNode's F Cost
                        NeighbourNode.cameFromThisNode = currentNode; // set đương mới
                
                        if (openHashSet.Contains(NeighbourNode) == false) // chưa chứa trong Open thì (lần đầu gặp node này)
                        {
                            // Ném vào để sau này tính toán
                            openMinHeapList.addNewNode(NeighbourNode);
                            openHashSet.Add(NeighbourNode);
                        }
                            
                    }
                }
            }
        }
        return null;
    }

    private void getNeighbourNodeList(PathNode_S currentNode)
    {
        List<PathNode_S> NeighbourNodeList = new List<PathNode_S>();
        int iCurrentNode = currentNode.i;
        int jCurrentNode = currentNode.j;

        // Up
        if (iCurrentNode - 1 >= 0)
            NeighbourNodeList.Add(grid.GetNodeType(iCurrentNode - 1, jCurrentNode));
        // Up Right
        if (iCurrentNode - 1 >= 0 && jCurrentNode + 1 < height)
            NeighbourNodeList.Add(grid.GetNodeType(iCurrentNode - 1, jCurrentNode + 1));
        // Right
        if (jCurrentNode + 1 < height)
            NeighbourNodeList.Add(grid.GetNodeType(iCurrentNode, jCurrentNode + 1));
        // Down Right
        if (iCurrentNode + 1 < width && jCurrentNode + 1 < height)
            NeighbourNodeList.Add(grid.GetNodeType(iCurrentNode + 1, jCurrentNode + 1));
        // Down
        if (iCurrentNode + 1 < width)
            NeighbourNodeList.Add(grid.GetNodeType(iCurrentNode + 1, jCurrentNode));
        // Down Left
        if (iCurrentNode + 1 < width && jCurrentNode - 1 >= 0)
            NeighbourNodeList.Add(grid.GetNodeType(iCurrentNode + 1, jCurrentNode - 1));
        // Left
        if (jCurrentNode - 1 >= 0)
            NeighbourNodeList.Add(grid.GetNodeType(iCurrentNode, jCurrentNode - 1));
        // Up Left
        if (iCurrentNode - 1 >= 0 && jCurrentNode - 1 >= 0)
            NeighbourNodeList.Add(grid.GetNodeType(iCurrentNode - 1, jCurrentNode - 1));

        currentNode.NeighboursList = NeighbourNodeList;
    }

    public List<PathNode_S> calculatePath(PathNode_S endNode)
    {
        List<PathNode_S> path = new List<PathNode_S>();
        PathNode_S nextNode = endNode;
        while (nextNode != null)
        {
            path.Add(nextNode);
            nextNode = nextNode.cameFromThisNode;
            if (nextNode == null)
            {
                break;
            }
        }
        path.Reverse();
        return path;
    }

    public int calculateDistanceBtwTwoNode(PathNode_S a, PathNode_S b) // tính khoảng cách hai Node bỏ qua chướng ngại vật
    {
        // maybe get ReferenceNUllExceptionError vì b có thể là một Node bên ngoài khi click chuột bên ngoài grid, a thì 0, 0 rồi không nói
        if (a.i >= 0 && a.i < width && a.j >= 0 && a.j < height && b.i >= 0 && b.i < width && b.j >= 0 && b.j < height)
        {
            int xDistance = Mathf.Abs(a.i - b.i);
            int yDistance = Mathf.Abs(a.j - b.j);
            // vì đi chéo mỗi lần sẽ mất 1 x và 1 y => số lượng ô di chuyển sẽ ít đi => ít chi phí (dù 1 lần đi chéo tốn chi phí hơn 1 lần đi thẳng, nhưng số lượng ô cần đi ít hơn)
            // số lượng ô đi != chi phí đi tới 1 ô theo thẳng hoặc chéo
            int DiagonalMove = Mathf.Min(xDistance, yDistance); // tính số lần đi chéo (vì đi chéo sẽ tới ô cần tới nhanh hơn đi thẳng thì đi tốn chi phí (mặc dù đi chéo tốn chi phí hơn nhưng số lượng ô cần đi ít hơn))
            int StraightMove = Mathf.Abs(xDistance - yDistance); // sau khi đi hết chéo thì HIỆU số còn lại chính là số lần BẮT BUỘC phải đi thẳng
            return DiagonalMove * MOVE_DIAGONAL_COST + StraightMove * MOVE_STRAIGHT_COST;
        }
        else
        {
            // Debug.Log(int.MaxValue);
            return int.MaxValue; // maybe get error
        }
    }

    private PathNode_S getLowestFCostOnOpenList(List<PathNode_S> pathNodeList) // <-- Tối ưu hàm này (đã thay bằng min heap)
    {
        PathNode_S LowestFCostPathNode = pathNodeList[0];
        for (int i = 0; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost < LowestFCostPathNode.fCost)
            {
                LowestFCostPathNode = pathNodeList[i];
            }
        }
        return LowestFCostPathNode;
    }

    public PathNode_S createNewPathNode_S(Grid_S<PathNode_S> grid, int i, int j) // hàm này tương tự hàm lambda bên trên
    {
        PathNode_S newPathNode = new PathNode_S(grid, i, j);
        return newPathNode;
    }

    public Grid_S<PathNode_S> getGrid()
    {
        return this.grid;
    }

    public void setBlockNode()
    {
        Vector3 mouseWorldPosition = UtilsClass.getMouseWorldPosition();
        grid.worldPosToIJPos(mouseWorldPosition, out int i, out int j);
        if (i >= 0 && i < width && j >= 0 && j < height)
        {
            // Debug.Log("Block node at i:" + i + " ; j:" + j);
            grid.GetNodeType(i, j).setIsWalkable(false);
            grid.SetDebugArr(i, j);
        }
    }
    public void setBlockNodeByGridPosition(int i, int j)
    {
        if (i >= 0 && i < width && j >= 0 && j < height)
        {
            // Debug.Log("Block node at i:" + i + " ; j:" + j);
            grid.GetNodeType(i, j).setIsWalkable(false);
            grid.SetDebugArr(i, j);
        }
    }

    public void setReBlockNode()
    {
        Vector3 mouseWorldPosition = UtilsClass.getMouseWorldPosition();
        // Grid_S<PathNode_S> tmpGrid = grid; // cần gì thằng này, ? lôi thẳng grid ra cũng được mà 
        grid.worldPosToIJPos(mouseWorldPosition, out int i, out int j);
        PathNode_S PathNode = grid.GetNodeType(i, j);
        if (grid.GetDebugArr(i, j) == "X")
        {
            grid.SetNormallyDebugArr(i, j);
            PathNode.setIsWalkable(true);
            removeNodeFromeCloseList(PathNode);
        }
    }
    public void setReBlockNodeByGridPosition(int i, int j)
    {
        if (i >= 0 && i < width && j >= 0 && j < height)
        {
            PathNode_S PathNode = grid.GetNodeType(i, j);
            if (grid.GetDebugArr(i, j) == "X")
            {
                grid.SetNormallyDebugArr(i, j);
                PathNode.setIsWalkable(true);
                removeNodeFromeCloseList(PathNode);
            }
        }        
    }

    public void removeNodeFromeCloseList(PathNode_S PathNode)
    {
        if (closeHashSet != null && closeHashSet.Contains(PathNode)) // maybe get error NullReference ??? 
        {
            closeHashSet.Remove(PathNode);
        }
    }

    public Vector3 getRootLocation()
    {
        return SetRootLocation;
    }

    private bool IsStandable(PathNode_S pathNode_S)
    {
        if(pathNode_S == null) return false;
        PathNode_S NodeBelow = grid.getNodeTypeByGridPosition(pathNode_S.i, pathNode_S.j-1);
        return NodeBelow != null && NodeBelow.getIsWalkable() == false;
    }

    private bool TryGetGroundNodeBelow(int endI, int endJ, out PathNode_S groundNode)
    {
        groundNode = null;

        PathNode_S startNode = grid.GetNodeType(endI, endJ);

        if(startNode != null && IsStandable(startNode) == true)
        {
            groundNode = startNode;
            return true;
        }

        for(int j = endJ -1 ; j >= 0 ; j--)
        {
            PathNode_S node = grid.getNodeTypeByGridPosition(endI, j);
            
            // C1: dò tìm Node Standable đầu tiên dưới Node endI, endJ
            // if(node == null) continue;

            // if(IsStandable(node) == true)
            // {
            //     groundNode = node;
            //     return true;
            // }

            // C2: tìm  Node IsWakable == false đầu tiên
            if (node.getIsWalkable() == false)
            {
                PathNode_S standNode = grid.getNodeTypeByGridPosition(endI, j+1);
                if(IsStandable(standNode) == true)
                {
                    groundNode = standNode;
                    return true;
                }
            }
        }
        return false;

    }
}

// REFACTOR CODE