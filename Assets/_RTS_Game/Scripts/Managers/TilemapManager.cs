using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : SingletonManager<TilemapManager>
{
    [SerializeField] private Tilemap m_walkableTilemap;
    [SerializeField] private Tilemap m_overlayTilemap;
    [SerializeField] private Tilemap[] m_unreachableTilemaps;
    private Color m_highlightColor = new Color(0, 0.8f, 1, 0.4f);
    private Color m_blockColor = new Color(1f, 0.2f, 0, 0.8f);
    private Sprite m_placeholderTileSprite;
    private Vector3Int[] m_highlightPositions;

    public Tilemap PathFindingTileMap => m_walkableTilemap;

    [SerializeField] private Transform warriorPos, builderPos;
    PathFinding m_pathFiding;


    protected override void Awake()
    {
        base.Awake();

        m_placeholderTileSprite = Resources.Load<Sprite>("Images/PlaceholderTileSprite");
    }

    private void Update()
    {
        m_pathFiding.FindPath(warriorPos.position, builderPos.position);
    }

    private void Start()
    {
        m_pathFiding = new PathFinding(this);
    }

    public bool IsInWalkable(Vector3Int tilePosition)
    {
        return m_walkableTilemap.HasTile(tilePosition);
    }

    public bool IsInUnreachableTileMap(Vector3Int tilePosition)
    {
        foreach (Tilemap tilemap in m_unreachableTilemaps)
        {
            if (tilemap.HasTile(tilePosition)) return true;
        }

        return false;
    }

    public bool IsBlockByHumanoidUnit(Vector3Int tilePosition)
    {
        Vector3 tileSize = m_walkableTilemap.cellSize;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(tilePosition + tileSize * 0.5f, tileSize * 0.5f, 0);

        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent(out Unit unit))
            {
                if (unit is HumanoidUnit) return true;
            }
        }

        return false;
    }

    public void ClearHighlightTiles()
    {
        if (m_highlightPositions == null) return;

        foreach (Vector3Int tilePosition in m_highlightPositions)
        {
            m_overlayTilemap.SetTile(tilePosition, null);
        }
    }

    public Vector3Int[] HighlightTiles(Vector3Int outlinePosition, Vector2Int size, Vector3Int offset)
    {
        Vector3Int pivotPosition = outlinePosition + offset;

        ClearHighlightTiles();
        m_highlightPositions = new Vector3Int[size.x * size.y];

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                m_highlightPositions[x + y * size.x] = new Vector3Int(pivotPosition.x + x, pivotPosition.y + y);
            }
        }

        foreach (Vector3Int tilePosition in m_highlightPositions)
        {
            Tile tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = m_placeholderTileSprite;
            tile.color = CanPlaceTile(tilePosition) ? m_highlightColor : m_blockColor;

            m_overlayTilemap.SetTile(tilePosition, tile);
        }

        return m_highlightPositions;
    }

    public bool CanWalkAtTile(Vector3Int tilePosition) => IsInWalkable(tilePosition) && !IsInUnreachableTileMap(tilePosition);
    public bool CanPlaceTile(Vector3Int tilePosition) => IsInWalkable(tilePosition) && !IsBlockByHumanoidUnit(tilePosition) && !IsInUnreachableTileMap(tilePosition);
}