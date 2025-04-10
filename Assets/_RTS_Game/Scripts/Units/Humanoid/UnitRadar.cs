using System;
using UnityEngine;

public class UnitRadar : MonoBehaviour, IActionNode
{
    [SerializeField] private EUnitClass[] m_unitClass;
    [SerializeField] private float m_objectDetectionRadius;
    [SerializeField] private float m_unitDetectionCheckRate;
    private float m_nextUnitDetectionTime;
    private Unit m_detectedUnit;
    public bool Enabled = true;

    public event Action<Unit> OnScanned;
    private void Update()
    {
        if (!Enabled) return;
        Scan(OnScanned);
    }

    private void Scan(Action<Unit> OnScanned)
    {
        if (Time.time < m_nextUnitDetectionTime) return;

        m_nextUnitDetectionTime = Time.time + m_unitDetectionCheckRate;

        if (TryFindClosestFoe(out Unit closestUnit)) { Debug.Log($"{this.gameObject.name} detect unit: {closestUnit.gameObject.name}"); }

        m_detectedUnit = closestUnit;
        OnScanned?.Invoke(closestUnit);
    }

    private bool TryFindClosestFoe(out Unit foe)
    {
        foe = null;

        if (m_unitClass == null) return false;

        foreach (EUnitClass unitClass in m_unitClass)
        {
            float detectRadius = unitClass == EUnitClass.KING ? float.MaxValue : m_objectDetectionRadius;

            foe = GameManager.Instance.FindClosestUnit(this.transform.position, detectRadius, unitClass);

            if (foe != null) break;
        }

        return foe != null;
    }

    public EStatusNode Execute(Blackboard blackboard, Action onSuccess)
    {
        bool hasUnit = m_detectedUnit != null;
        if (!hasUnit) return EStatusNode.FAILURE;

        blackboard.SetValue(Blackboard.CLASS_TARGET, m_detectedUnit);
        return EStatusNode.SUCCESS;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1, 0.25f);
        Gizmos.DrawSphere(this.transform.position, m_objectDetectionRadius);
    }
}