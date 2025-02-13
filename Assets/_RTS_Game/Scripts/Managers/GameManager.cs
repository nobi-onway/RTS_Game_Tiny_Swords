using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : SingletonManager<GameManager>
{
    [SerializeField] private Tilemap m_walkableTilemap;
    [SerializeField] private Tilemap m_overlayTilemap;
    [SerializeField] private Tilemap[] m_unreachableTilemap;
    public Tilemap WalkableTilemap => m_walkableTilemap;
    public Tilemap OverlayTilemap => m_overlayTilemap;
    public Tilemap[] UnreachableTilemap => m_unreachableTilemap;

    private Unit m_activeUnit;
    private bool m_hasActiveUnit => m_activeUnit != null;

    public event Action<Vector2> OnMoveActiveUnit;
    public event Action<Unit> OnSelectUnit;
    public event Action OnDeselectUnit;

    private BuildingUnit m_activeBuildingUnit;
    public event Action OnCancelSelectBuildingUnit;
    public event Action<BuildingSO> OnSelectBuildingUnit;

    public void PlaceActiveBuildingUnit()
    {
        if (m_activeBuildingUnit == null) return;

        if (!m_activeBuildingUnit.TryStartBuildProgress())
        {
            Destroy(m_activeBuildingUnit.gameObject);
        }

        OnCancelSelectBuildingUnit?.Invoke();
        m_activeBuildingUnit = null;
    }

    public void SelectNewBuildingUnit(BuildingUnit buildingUnit, BuildingSO buildingSO)
    {
        m_activeBuildingUnit = buildingUnit;
        OnSelectBuildingUnit?.Invoke(buildingSO);
    }

    public void MoveActiveUnitTo(Vector2 position)
    {
        if (!m_hasActiveUnit) return;

        m_activeUnit.MoveTo(position);
        OnMoveActiveUnit?.Invoke(position);
    }
    public void SelectUnit(Unit unit)
    {
        if (unit == m_activeUnit)
        {
            CancelActiveUnit();
            return;
        }

        SelectNewUnit(unit);
    }

    private void SelectNewUnit(Unit unit)
    {
        if (m_hasActiveUnit)
        {
            m_activeUnit.Deselect();
        }

        SetActiveUnit(unit);
        m_activeUnit.Select();
    }

    private void CancelActiveUnit()
    {
        m_activeUnit.Deselect();
        SetActiveUnit(null);
    }

    private void SetActiveUnit(Unit unit)
    {
        m_activeUnit = unit;

        if (m_activeUnit != null) OnSelectUnit?.Invoke(unit);
        if (m_activeUnit == null) OnDeselectUnit?.Invoke();
    }
}