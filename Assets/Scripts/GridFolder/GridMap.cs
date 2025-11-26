using UnityEditor.Experimental.GraphView;
using UnityEngine;
using ZonBon.Utils;

public class GridMap : MonoBehaviour
{
    public PathFinding pathFinding;
    public BlockSaveLoadManager refBlockSaveLoadManager;
    private void Awake()
    {
        pathFinding = new PathFinding(60, 30, 1, new Vector3(-16, -6, 0));
    }
    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.N))
        {
            Vector3 mouseWorldPosition = UtilsClass.getMouseWorldPosition();
            pathFinding.setBlockNode();
            pathFinding.getGrid().worldPosToIJPos(mouseWorldPosition, out int i, out int j);
            refBlockSaveLoadManager.addNodeToBlockNodeGridPositionList(i, j);
        }
        if (Input.GetMouseButtonDown(2) && Input.GetKey(KeyCode.N))
        {
            Vector3 mouseWorldPosition = UtilsClass.getMouseWorldPosition();
            pathFinding.setReBlockNode();
            pathFinding.getGrid().worldPosToIJPos(mouseWorldPosition, out int i, out int j);
            refBlockSaveLoadManager.removeNodeFromBlockNodeGridPositionList(i, j);
        }
#endif
    }
    
    public PathFinding getPathFinding()
    {
        return this.pathFinding;
    }

    public Grid_S<PathNode_S> getGrid()
    {
        return this.pathFinding.getGrid();
    }

    public bool getIsWalkableByGridPosition(int i, int j)
    {
        Grid_S<PathNode_S> tmpGrid = pathFinding.getGrid();
        PathNode_S tmpPathNode_S = tmpGrid.getNodeTypeByGridPosition(i, j);
        return tmpPathNode_S.getIsWalkable();
    }
}
