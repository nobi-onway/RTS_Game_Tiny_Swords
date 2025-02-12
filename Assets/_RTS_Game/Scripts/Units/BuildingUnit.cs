using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingUnit : MonoBehaviour
{
    private BuildingSO m_buildingSO;
    private SpriteRenderer m_spriteRenderer;
    private Vector3Int[] m_highlightPositions;
    private Tilemap m_walkableTilemap;
    private Tilemap m_overlayTilemap;
    private Tilemap[] m_unreachableTilemap;
    private Sprite m_placeholderTileSprite;

    private Color m_highlightColor = new Color(0, 0.8f, 1, 0.4f);
    private Color m_blockColor = new Color(1f, 0.2f, 0, 0.8f);

    private float? m_buildingProgress;

    private void Update()
    {
        if (m_buildingProgress.HasValue) return;

        if (InputManager.Instance.TryGetHoldWorldPosition(out Vector2 worldPosition))
        {
            Vector3Int snapPosition = GridUtils.SnapToGrid(worldPosition);

            this.transform.position = snapPosition;
            HighlightTiles(snapPosition);
        }
    }

    public bool TryStartBuildProgress()
    {
        ClearHighlightTiles();

        foreach (Vector3Int tilePosition in m_highlightPositions)
        {
            if (!CanPlaceTile(tilePosition)) return false;
        }

        m_buildingProgress = 0;
        return true;
    }

    public void SetUpBySO(BuildingSO buildingSO, Tilemap walkableTilemap, Tilemap overlayTilemap, Tilemap[] unreachalbeTilemap)
    {
        m_buildingSO = buildingSO;
        m_walkableTilemap = walkableTilemap;
        m_overlayTilemap = overlayTilemap;
        m_unreachableTilemap = unreachalbeTilemap;

        m_placeholderTileSprite = Resources.Load<Sprite>("Images/PlaceholderTileSprite");

        m_spriteRenderer = this.AddComponent<SpriteRenderer>();

        m_spriteRenderer.sprite = m_buildingSO.PlacementSprite;
        m_spriteRenderer.color = new Color(1, 1, 1, 0.5f);
        m_spriteRenderer.sortingLayerName = "Unit";
    }

    private void HighlightTiles(Vector3Int outlinePosition)
    {
        Vector2Int buildingSize = m_buildingSO.BuildingSize;
        Vector3Int pivotPosition = outlinePosition + m_buildingSO.BuildingOffset;

        ClearHighlightTiles();
        m_highlightPositions = new Vector3Int[buildingSize.x * buildingSize.y];

        for (int x = 0; x < buildingSize.x; x++)
        {
            for (int y = 0; y < buildingSize.y; y++)
            {
                m_highlightPositions[x + y * buildingSize.x] = new Vector3Int(pivotPosition.x + x, pivotPosition.y + y);
            }
        }

        foreach (Vector3Int tilePosition in m_highlightPositions)
        {
            Tile tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = m_placeholderTileSprite;
            tile.color = CanPlaceTile(tilePosition) ? m_highlightColor : m_blockColor;

            m_overlayTilemap.SetTile(tilePosition, tile);
        }
    }

    private void ClearHighlightTiles()
    {
        if (m_highlightPositions == null) return;

        foreach (Vector3Int tilePosition in m_highlightPositions)
        {
            m_overlayTilemap.SetTile(tilePosition, null);
        }
    }

    private bool CanPlaceTile(Vector3Int tilePosition)
    {
        return m_walkableTilemap.HasTile(tilePosition) && !IsInUnreachableTilemap(tilePosition) && !IsBlockByUnit(tilePosition);
    }

    private bool IsInUnreachableTilemap(Vector3Int tilePosition)
    {
        foreach (Tilemap tilemap in m_unreachableTilemap)
        {
            if (tilemap.HasTile(tilePosition)) return true;
        }

        return false;
    }

    private bool IsBlockByUnit(Vector3Int tilePosition)
    {
        Vector3 tileSize = m_walkableTilemap.cellSize;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(tilePosition + tileSize * 0.5f, tileSize * 0.5f, 0);

        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent(out Unit unit)) return true;
        }

        return false;
    }
}