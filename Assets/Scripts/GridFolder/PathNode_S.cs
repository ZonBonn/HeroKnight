using System;
using System.Collections.Generic;

public class PathNode_S : IComparable<PathNode_S>
{
    Grid_S<PathNode_S> grid; // lưu lưới mà Node này thuộc về
    public int i;
    public int j;
    public List<PathNode_S> NeighboursList;

    public int gCost;
    public int hCost; // chi phí từ startNode tới currentNode
    public int fCost; // chi phí (ước tính == heuristic) từ currentNode tới endNode
    public PathNode_S cameFromThisNode;
    public bool isWalkable;

    public PathNode_S(Grid_S<PathNode_S> grid, int i, int j)
    {
        this.grid = grid;
        this.i = i;
        this.j = j;
        this.isWalkable = true;
        NeighboursList = new List<PathNode_S>();
    }

    public void calculateFCost()
    {
        this.fCost = gCost + hCost;
    }

    public void setIsWalkable(bool isWalkable)
    {
        this.isWalkable = isWalkable;
    }

    public override string ToString()
    {
        return (i + "," + j).ToString();
    }

    // hỗ trợ cách so sánh hai PathNode_S ở các file .cs khác
    public int CompareTo(PathNode_S other)
    {
        return fCost.CompareTo(other.fCost);
        // -1: this.fCost < other.fCost
        // 0: this.fCost == other.fCost
        // 1: this.fCost > other.fCost
    }
    // override vì dữ liệu bên trong là một class nên HashSet không biết nên sắp xếp như nào vì thế phải override cách sắp xếp
    public override bool Equals(object obj)
    {
        if (obj is PathNode_S other)
            return this.i == other.i && this.j == other.j;
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(i, j);
    }

    public bool getIsWalkable()
    {
        return isWalkable;
    }
}
