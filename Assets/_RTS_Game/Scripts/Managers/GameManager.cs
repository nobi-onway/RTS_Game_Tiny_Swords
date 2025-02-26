using System;
using UnityEngine;

public class GameManager : SingletonManager<GameManager>
{
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

        if (!m_activeBuildingUnit.TryStartBuildProgress(m_activeUnit as WorkerUnit))
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

    public void ExecuteActiveUnit(Vector2 position)
    {
        if (!m_hasActiveUnit) return;

        if (m_activeUnit is HumanoidUnit)
        {
            ((HumanoidUnit)m_activeUnit).MoveTo(position);
            OnMoveActiveUnit?.Invoke(position);
        }
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

    void OnGUI()
    {
        if (m_activeUnit)
        {
            if (!m_activeUnit.TryGetComponent(out WorkerUnit workerUnit)) return;

            GUI.Label(new Rect(20, 120, 200, 20), "State: " + workerUnit.CurrentState, new GUIStyle { fontSize = 30 });
            GUI.Label(new Rect(20, 160, 200, 20), "Task: " + workerUnit.CurrentTask, new GUIStyle { fontSize = 30 });
        }
    }
}