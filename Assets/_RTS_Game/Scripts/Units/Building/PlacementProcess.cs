using UnityEngine;
using UnityEngine.Tilemaps;

public class PlacementProcess
{
    private SpriteRenderer m_spriteRenderer;
    private Tilemap m_walkableTilemap;
    private Tilemap m_overlayTilemap;
    private Tilemap[] m_unreachableTilemaps;
    private Vector3Int[] m_highlightPositions;
    private Sprite m_placeholderTileSprite;

    private Color m_highlightColor = new Color(0, 0.8f, 1, 0.4f);
    private Color m_blockColor = new Color(1f, 0.2f, 0, 0.8f);

    private GameObject m_placementOutline;

    public PlacementProcess(Transform parent, Sprite sprite, ref SpriteRenderer spriteRenderer, Tilemap walkableTilemap, Tilemap overlayTilemap, Tilemap[] unreachalbeTilemaps)
    {
        m_walkableTilemap = walkableTilemap;
        m_overlayTilemap = overlayTilemap;
        m_unreachableTilemaps = unreachalbeTilemaps;

        m_placeholderTileSprite = Resources.Load<Sprite>("Images/PlaceholderTileSprite");

        InitVisual(parent, sprite, ref spriteRenderer);
    }

    public void Update(BuildingSO buildingSO)
    {
        HighlightTiles(GridUtils.SnapToGrid(m_placementOutline.transform.parent.position), buildingSO.BuildingSize, buildingSO.BuildingOffset);

        if (InputManager.Instance.TryGetHoldWorldPosition(out Vector2 worldPosition))
        {
            Vector3Int snapPosition = GridUtils.SnapToGrid(worldPosition);
            m_placementOutline.transform.parent.position = snapPosition;
        }
    }

    public bool TryPlaceBuilding()
    {
        ClearHighlightTiles();

        foreach (Vector3Int tilePosition in m_highlightPositions)
        {
            if (!CanPlaceTile(tilePosition)) return false;
        }

        m_spriteRenderer.enabled = false;
        return true;
    }

    private void InitVisual(Transform parent, Sprite sprite, ref SpriteRenderer spriteRenderer)
    {
        m_placementOutline = new GameObject("Placement_Process");
        m_placementOutline.transform.SetParent(parent);
        m_placementOutline.transform.localPosition = Vector3.zero;

        m_spriteRenderer = m_placementOutline.AddComponent<SpriteRenderer>();
        m_spriteRenderer.sprite = sprite;
        m_spriteRenderer.color = new Color(1, 1, 1, 0.5f);
        m_spriteRenderer.sortingLayerName = "Unit";

        spriteRenderer = m_spriteRenderer;
    }

    private void HighlightTiles(Vector3Int outlinePosition, Vector2Int buildingSize, Vector3Int buildingOffset)
    {
        Vector3Int pivotPosition = outlinePosition + buildingOffset;

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
        foreach (Tilemap tilemap in m_unreachableTilemaps)
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
            if (collider.TryGetComponent(out Unit unit))
            {
                if (unit.gameObject != this.m_placementOutline.transform.parent.gameObject) return true;
            }
        }

        return false;
    }
}