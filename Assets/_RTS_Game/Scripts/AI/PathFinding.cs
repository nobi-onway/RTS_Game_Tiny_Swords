using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PathFinding
{
    private int m_width;
    private int m_height;
    private Vector3Int m_gridOffset;
    private TilemapManager m_tilemapManager;
    private PathNode[,] m_grid;
    public PathNode[,] Grid => m_grid;

    public PathFinding(TilemapManager tilemapManager)
    {
        m_tilemapManager = tilemapManager;
        m_tilemapManager.PathFindingTileMap.CompressBounds();

        BoundsInt bounds = m_tilemapManager.PathFindingTileMap.cellBounds;

        m_width = bounds.size.x;
        m_height = bounds.size.y;
        m_gridOffset = m_tilemapManager.PathFindingTileMap.cellBounds.min;

        InitializeGrid();
    }

    private void InitializeGrid()
    {
        m_grid = new PathNode[m_width, m_height];

        Vector3 cellSize = m_tilemapManager.PathFindingTileMap.cellSize;

        for (int x = 0; x < m_width; x++)
        {
            for (int y = 0; y < m_height; y++)
            {
                Vector3Int nodeLeftBottomPosition = new Vector3Int(x + m_gridOffset.x, y + m_gridOffset.y);
                bool isWalkable = m_tilemapManager.CanWalkAtTile(nodeLeftBottomPosition);
                PathNode node = new PathNode(nodeLeftBottomPosition, cellSize, isWalkable);

                m_grid[x, y] = node;
            }
        }
    }

    public void FindPath(Vector3 startPosition, Vector3 endPosition)
    {
        PathNode startNode = FindNode(startPosition);
        PathNode endNode = FindNode(endPosition);

        Debug.Log("Start Node: " + startNode);
        Debug.Log("End Node: " + endNode);
    }

    private PathNode FindNode(Vector3 position)
    {
        Vector3Int gridPosition = GridUtils.SnapToGrid(position);

        int gridX = gridPosition.x - m_gridOffset.x;
        int gridY = gridPosition.y - m_gridOffset.y;

        if (gridX > 0 && gridX < m_width && gridY > 0 && gridY < m_height)
        {
            return m_grid[gridX, gridY];
        }

        Debug.Log("Node not found");
        return null;
    }
}