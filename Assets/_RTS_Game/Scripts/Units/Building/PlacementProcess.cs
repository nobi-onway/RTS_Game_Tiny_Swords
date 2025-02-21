using UnityEngine;

public class PlacementProcess
{
    private SpriteRenderer m_spriteRenderer;
    private GameObject m_placementOutline;
    private TilemapManager m_tilemapManager;

    private Vector3Int[] m_placementPositions;

    public PlacementProcess(Transform parent, Sprite sprite, ref SpriteRenderer spriteRenderer, TilemapManager tilemapManager)
    {
        m_tilemapManager = tilemapManager;

        InitVisual(parent, sprite, ref spriteRenderer);
    }

    public void Update(BuildingSO buildingSO)
    {
        HighlightPlaceTiles(buildingSO);

        if (InputManager.Instance.TryGetHoldWorldPosition(out Vector2 worldPosition))
        {
            Vector3Int snapPosition = GridUtils.SnapToGrid(worldPosition);
            m_placementOutline.transform.parent.position = snapPosition;
        }
    }

    public bool TryPlaceBuilding()
    {
        ClearHighlightPlaceTile();

        foreach (Vector3Int tilePosition in m_placementPositions)
        {
            if (!m_tilemapManager.CanPlaceTile(tilePosition)) return false;
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

    private void HighlightPlaceTiles(BuildingSO buildingSO)
    {
        m_placementPositions = m_tilemapManager.HighlightTiles(
                                    GridUtils.SnapToGrid(m_placementOutline.transform.parent.position),
                                    buildingSO.BuildingSize,
                                    buildingSO.BuildingOffset
                                );
    }

    private void ClearHighlightPlaceTile() => m_tilemapManager.ClearHighlightTiles();
}