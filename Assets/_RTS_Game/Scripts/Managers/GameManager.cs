using System.Collections.Generic;
using System.Linq;
using UnityEditor.Callbacks;
using UnityEngine;

public class GameManager : SingletonManager<GameManager>
{
    private Unit m_activeUnit;
    private bool m_hasActiveUnit => m_activeUnit != null;
    private BuildingUnit m_activeBuildingUnit;
    private List<Unit> m_playerUnits = new();
    private List<Unit> m_enemyUnits = new();
    private List<StructureUnit> m_structureUnits = new();

    private Dictionary<EUnitClass, List<Unit>> UnitListLookUp;

    [SerializeField] private Transform m_treeContainer;
    private Tree[] m_trees = new Tree[0];
    public Unit ActiveUnit => m_activeUnit;

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

    private Unit FindClosestUnit(List<Unit> units, Vector3 originalPosition, float maxDistance)
    {
        Unit closestUnit = null;
        float closestDistance = float.MaxValue;

        foreach (Unit unit in units)
        {
            if (!unit.Enable) continue;

            float distance = Vector3.Distance(originalPosition, unit.transform.position);
            if (distance <= maxDistance && distance < closestDistance)
            {
                closestUnit = unit;
                closestDistance = distance;
            }
        }

        return closestUnit;
    }

    public Unit FindClosestUnit(Vector3 originalPosition, float maxDistance, EUnitClass unitClass)
    {
        List<Unit> units = UnitListLookUp[unitClass];

        return FindClosestUnit(units, originalPosition, maxDistance);
    }
    public StructureUnit FindClosestStructureUnit(Vector3 originalPosition, float maxDistance)
    {
        List<Unit> units = m_structureUnits.Cast<Unit>().ToList();

        return FindClosestUnit(units, originalPosition, maxDistance) as StructureUnit;
    }

    public Tree FindClosestUnClaimedTree(Vector3 originalPosition)
    {
        Tree closestTree = null;
        float closestDistance = float.MaxValue;

        if (m_trees.Length == 0)
        {
            m_trees = new Tree[m_treeContainer.childCount];

            for (int i = 0; i < m_treeContainer.childCount; i++)
            {
                m_trees[i] = m_treeContainer.GetChild(i).GetComponent<Tree>();
            }
        }

        foreach (Tree tree in m_trees)
        {
            if (tree.IsExploited) continue;

            float distance = Vector3.Distance(originalPosition, tree.transform.position);
            if (distance < closestDistance)
            {
                closestTree = tree;
                closestDistance = distance;
            }
        }

        return closestTree;
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
    public void SendWorkerToChop(Tree tree)
    {
        if (!m_hasActiveUnit) return;
        if (m_activeUnit is not WorkerUnit workerUnit) return;

        tree.StartExploitBy(workerUnit);
    }
    public void RegisterUnit(Unit unit)
    {
        UnitListLookUp[unit.Class].Add(unit);
    }
    public void UnregisterUnit(Unit unit)
    {
        UnitListLookUp[unit.Class].Remove(unit);

        if (unit == m_activeUnit) CancelActiveUnit();
    }
    public void RegisterUnit(StructureUnit unit)
    {
        m_structureUnits.Add(unit);
    }

    public void UnregisterUnit(StructureUnit unit)
    {
        m_structureUnits.Remove(unit);

        if (unit == m_activeUnit) CancelActiveUnit();
    }

    public List<Unit> GetUnits(EUnitClass eUnitClass) => UnitListLookUp[eUnitClass];

    private void SelectNewUnit(Unit unit)
    {
        if (!IsRegisteredUnit(unit)) return;
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

    private bool IsRegisteredUnit(Unit unit) => UnitListLookUp[unit.Class].Contains(unit);

    void OnGUI()
    {
        if (m_activeUnit)
        {
            GUI.Label(new Rect(20, 120, 200, 20), "State: " + m_activeUnit.CurrentState, new GUIStyle { fontSize = 30 });
            // GUI.Label(new Rect(20, 160, 200, 20), "Task: " + m_activeUnit.CurrentTask, new GUIStyle { fontSize = 30 });
        }
    }
}