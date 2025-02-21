using UnityEngine;

public class PathNode
{
    public int x;
    public int y;
    public float centerX;
    public float centerY;
    public bool IsWalkable;

    public PathNode(Vector3Int position, Vector3 cellSize, bool isWalkable)
    {
        this.x = position.x;
        this.y = position.y;

        Vector3 halfCellSize = cellSize / 2;
        Vector3 nodeCenterPosition = position + halfCellSize;

        centerX = nodeCenterPosition.x;
        centerY = nodeCenterPosition.y;

        this.IsWalkable = isWalkable;
    }

    public override string ToString()
    {
        return $"x: {centerX}, y: {centerY}";
    }
}