using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonManager<GameManager>
{
    private Unit m_activeUnit;
    private bool m_hasActiveUnit => m_activeUnit != null;
    private BuildingUnit m_activeBuildingUnit;
    private List<Unit> m_playerUnits = new();
    private List<Unit> m_enemyUnits = new();

    private Dictionary<EUnitClass, List<Unit>> UnitListLookUp;

    protected override void Awake()
    {
        base.Awake();

        UnitListLookUp = new Dictionary<EUnitClass, List<Unit>>() {
            { EUnitClass.PLAYER, m_playerUnits },
            { EUnitClass.ENEMY, m_enemyUnits },
        };
    }

    public void PlaceActiveBuildingUnit()
    {
        if (m_activeBuildingUnit == null) return;

        if (!m_activeBuildingUnit.TryStartBuildProgress(m_activeUnit as WorkerUnit))
        {
            Destroy(m_activeBuildingUnit.gameObject);
        }

        m_activeBuildingUnit = null;
    }
    public void SelectNewBuildingUnit(BuildingUnit buildingUnit)
    {
        m_activeBuildingUnit = buildingUnit;
    }
    public void ExecuteActiveUnit(Vector2 position)
    {
        if (!m_hasActiveUnit) return;

        m_activeUnit.DoActionAt(position);
    }
    public Unit FindClosestUnit(Vector3 originalPosition, float maxDistance, EUnitClass unitClass)
    {
        List<Unit> units = UnitListLookUp[unitClass];

        Unit closestUnit = null;
        float closestDistance = float.MaxValue;

        foreach (Unit unit in units)
        {
            float distance = Vector3.Distance(originalPosition, unit.transform.position);
            if (distance <= maxDistance && distance < closestDistance)
            {
                closestUnit = unit;
                closestDistance = distance;
            }
        }

        return closestUnit;
    }
    public void SelectUnit(Unit unit)
    {
        if (unit == m_activeUnit)
        {
            CancelActiveUnit();
            return;
        }

        if (!m_hasActiveUnit || !m_activeUnit.TryInteractWithOtherUnit(unit))
        {
            SelectNewUnit(unit);
        }
    }
    public void RegisterUnit(Unit unit)
    {
        UnitListLookUp[unit.Class].Add(unit);
    }
    public void UnregisterUnit(Unit unit)
    {
        UnitListLookUp[unit.Class].Remove(unit);
    }
    private void SelectNewUnit(Unit unit)
    {
        if (!unit.TryGetComponent(out SelectableUnit selectableUnit)) return;

        CancelActiveUnit();

        SetActiveUnit(unit);
        selectableUnit.Select();
    }

    private void CancelActiveUnit()
    {
        if (!m_hasActiveUnit) return;
        if (!m_activeUnit.TryGetComponent(out SelectableUnit selectableUnit)) return;

        selectableUnit.Deselect();
        SetActiveUnit(null);
    }

    private void SetActiveUnit(Unit unit)
    {
        m_activeUnit = unit;
    }

    void OnGUI()
    {
        if (m_activeUnit)
        {
            GUI.Label(new Rect(20, 120, 200, 20), "State: " + m_activeUnit.CurrentState, new GUIStyle { fontSize = 30 });
            // GUI.Label(new Rect(20, 160, 200, 20), "Task: " + m_activeUnit.CurrentTask, new GUIStyle { fontSize = 30 });
        }
    }
}