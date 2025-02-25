using UnityEngine;

public class PathNode
{
    public int x;
    public int y;
    public float centerX;
    public float centerY;
    public bool isWalkable;

    public int gCost;
    public int hCost;
    public int fCost => gCost + hCost;
    public PathNode parent;

    public PathNode(Vector3Int position, Vector3 cellSize, bool isWalkable)
    {
        this.x = position.x;
        this.y = position.y;

        Vector3 halfCellSize = cellSize / 2;
        Vector3 nodeCenterPosition = position + halfCellSize;

        centerX = nodeCenterPosition.x;
        centerY = nodeCenterPosition.y;

        this.isWalkable = isWalkable;
    }

    public override string ToString()
    {
        return $"({x}, {y})";
    }
}