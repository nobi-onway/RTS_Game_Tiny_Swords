using UnityEngine;
public static class GridUtils
{
    public static Vector3Int SnapToGrid(Vector3 position)
    {
        return new Vector3Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y), 0);
    }
}