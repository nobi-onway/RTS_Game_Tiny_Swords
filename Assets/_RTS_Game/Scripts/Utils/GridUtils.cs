using UnityEngine;
public static class GridUtils
{
    public static Vector3 SnapToGrid(Vector3 position)
    {
        return new Vector3(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y), 0);
    }
}