using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathFinding
{
    private int m_width;
    private int m_height;
    private Vector3Int m_gridOffset;
    private PathNode[,] m_grid;
    private Func<Vector3Int, bool> CanWalkAtTile;

    public PathFinding(Tilemap tilemap, Func<Vector3Int, bool> CanWalkAtTile)
    {
        this.CanWalkAtTile = CanWalkAtTile;
        tilemap.CompressBounds();

        BoundsInt bounds = tilemap.cellBounds;

        m_width = bounds.size.x;
        m_height = bounds.size.y;
        m_gridOffset = tilemap.cellBounds.min;

        InitializeGrid(tilemap.cellSize);
    }

    private void InitializeGrid(Vector3 cellSize)
    {
        m_grid = new PathNode[m_width, m_height];

        for (int x = 0; x < m_width; x++)
        {
            for (int y = 0; y < m_height; y++)
            {
                Vector3Int nodeLeftBottomPosition = new Vector3Int(x + m_gridOffset.x, y + m_gridOffset.y);
                bool isWalkable = CanWalkAtTile(nodeLeftBottomPosition);
                PathNode node = new PathNode(nodeLeftBottomPosition, cellSize, isWalkable);

                m_grid[x, y] = node;
            }
        }
    }

    public List<Vector3> FindPath(Vector3 startPosition, Vector3 endPosition)
    {
        PathNode startNode = FindNode(startPosition);
        PathNode endNode = FindNode(endPosition);

        if (startNode == null || endNode == null) return null;

        List<PathNode> openList = new();
        HashSet<PathNode> closeList = new();

        openList.Add(startNode);

        PathNode closestNode = startNode;
        int closestDistanceToEnd = GetDistance(endNode, closestNode);

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);

            if (currentNode == endNode)
            {
                List<Vector3> retracePath = RetracePath(startNode, endNode, startPosition);
                ResetNodes(openList, closeList);
                return retracePath;
            }

            openList.Remove(currentNode);
            closeList.Add(currentNode);

            foreach (PathNode neighborNode in GetNeighbors(currentNode))
            {
                if (!neighborNode.isWalkable || closeList.Contains(neighborNode)) continue;

                int tentativeG = currentNode.gCost + GetDistance(currentNode, neighborNode);

                if (tentativeG < neighborNode.gCost || !openList.Contains(neighborNode))
                {
                    int distance = GetDistance(neighborNode, endNode);

                    neighborNode.gCost = tentativeG;
                    neighborNode.hCost = distance;
                    neighborNode.parent = currentNode;

                    if (distance < closestDistanceToEnd)
                    {
                        closestNode = neighborNode;
                        closestDistanceToEnd = distance;
                    }

                    if (!openList.Contains(neighborNode))
                    {
                        openList.Add(neighborNode);
                    }
                }
            }
        }

        List<Vector3> unfinishedPath = RetracePath(startNode, closestNode, startPosition);
        ResetNodes(openList, closeList);
        return unfinishedPath;
    }

    public void UpdateNodesInArea(Vector3Int startPosition, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int xPosition = startPosition.x + x;
                int yPosition = startPosition.y + y;

                int gridX = xPosition - m_gridOffset.x;
                int gridY = yPosition - m_gridOffset.y;

                if (gridX > 0 && gridY > 0 && gridX < m_width && gridY < m_height)
                {
                    PathNode node = m_grid[gridX, gridY];
                    Vector3Int tilePosition = new Vector3Int(node.x, node.y);
                    node.isWalkable = CanWalkAtTile(tilePosition);
                }
            }
        }
    }

    private void ResetNodes(List<PathNode> openList, HashSet<PathNode> closeList)
    {
        foreach (PathNode node in openList)
        {
            node.gCost = 0;
            node.hCost = 0;
            node.parent = null;
        }

        foreach (PathNode node in closeList)
        {
            node.gCost = 0;
            node.hCost = 0;
            node.parent = null;
        }

        openList.Clear();
        closeList.Clear();
    }

    private List<Vector3> RetracePath(PathNode startNode, PathNode endNode, Vector3 startPosition)
    {
        List<Vector3> path = new();
        PathNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(new Vector3(currentNode.centerX, currentNode.centerY));
            currentNode = currentNode.parent;
        }

        path.Add(startPosition);
        path.Reverse();

        return path;
    }

    private int GetDistance(PathNode startNode, PathNode endNode)
    {
        int dstX = Mathf.Abs(startNode.x - endNode.x);
        int dstY = Mathf.Abs(startNode.y - endNode.y);

        int diagonalMultiplier = dstX > dstY ? dstY : dstX;
        int horizontalMultiplier = Mathf.Abs(dstX - dstY);

        return 14 * diagonalMultiplier + 10 * horizontalMultiplier;
    }

    private List<PathNode> GetNeighbors(PathNode node)
    {
        List<PathNode> neighbors = new();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                int indexX = node.x + x - m_gridOffset.x;
                int indexY = node.y + y - m_gridOffset.y;

                bool isValid = indexX > 0 && indexX < m_width && indexY > 0 && indexY < m_height;

                if (!isValid) continue;

                neighbors.Add(m_grid[indexX, indexY]);
            }
        }

        return neighbors;
    }

    private PathNode GetLowestFCostNode(List<PathNode> list)
    {
        PathNode lowestNode = list[0];

        foreach (PathNode node in list)
        {
            if (node.fCost > lowestNode.fCost || (node.fCost == lowestNode.fCost && node.hCost > lowestNode.hCost)) continue;
            lowestNode = node;
        }

        return lowestNode;
    }

    public PathNode FindNode(Vector3 position)
    {
        Vector3Int gridPosition = GridUtils.SnapToGrid(position);

        int gridX = gridPosition.x - m_gridOffset.x;
        int gridY = gridPosition.y - m_gridOffset.y;

        if (gridX > 0 && gridX < m_width && gridY > 0 && gridY < m_height)
        {
            return m_grid[gridX, gridY];
        }

        return null;
    }

    public PathNode FindClosestPathNode(Vector3 position, Vector3 to)
    {
        PathNode pathNode = FindNode(position);
        PathNode endNode = FindNode(to);
        List<PathNode> neighbors = GetNeighbors(pathNode);

        float closestDistanceToEnd = GetDistance(pathNode, endNode);

        foreach (PathNode node in neighbors)
        {
            if (!node.isWalkable) continue;

            float distance = GetDistance(node, endNode);

            if (distance < closestDistanceToEnd)
            {
                pathNode = node;
                closestDistanceToEnd = distance;
            }
        }

        return pathNode;
    }
}