using System;
using UnityEngine;

public class GameManager : SingletonManager<GameManager>
{
    #region Unit
    private Unit m_activeUnit;
    private bool m_hasActiveUnit => m_activeUnit != null;

    public event Action<Vector2> OnMoveActiveUnit;
    public event Action<Unit> OnSelectUnit;
    public event Action OnDeselectUnit;
    #endregion

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