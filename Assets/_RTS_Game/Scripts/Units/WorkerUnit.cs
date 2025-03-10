using UnityEngine;

public enum EWorkerTask { None, Build }

public class WorkerUnit : HumanoidUnit
{
    [SerializeField]
    private BuildingUnit m_targetBuilding => target?.GetComponent<BuildingUnit>();

    private EnumSystem<EWorkerTask> m_taskSystem = new();

    public EWorkerTask CurrentTask => m_taskSystem.Value;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_stateSystem.OnValueChange += OnStateChange;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        m_stateSystem.OnValueChange -= OnStateChange;
    }

    protected override void UpdateBehavior()
    {
        if (m_targetBuilding == null) return;

        DetectBuilding();

        if (m_stateSystem.IsCurrentValue(EUnitState.BUILDING))
        {
            m_targetBuilding?.UpdateBuildingProgress(Time.fixedDeltaTime);
        }
    }
    protected override void ResetAction()
    {
        m_targetBuilding?.UnassignWorkerUnit(this);
    }
    private void OnStateChange(EUnitState newState)
    {
        animator.SetBool("IsBuilding", newState == EUnitState.BUILDING);
    }
    private void DetectBuilding()
    {
        if (!IsCloseObject(out BuildingUnit buildingUnit)) return;
        if (buildingUnit != m_targetBuilding) return;

        m_stateSystem.SetValue(EUnitState.IDLE, EUnitState.BUILDING);
    }

    public void AssignToBuildProcess(BuildingUnit targetBuilding)
    {
        if (targetBuilding == null) return;

        SetTarget(targetBuilding);
        MoveTo(targetBuilding.transform.position + Vector3.up * 1.5f);

        m_taskSystem.SetValue(EWorkerTask.Build);
    }

    public void UnassignFromBuildProcess()
    {
        SetTarget(null);
        m_taskSystem.SetValue(EWorkerTask.None);
        m_stateSystem.SetValue(EUnitState.BUILDING, EUnitState.IDLE);
    }

    public override bool TryInteractWithOtherUnit(Unit unit)
    {
        if (unit is not BuildingUnit buildingUnit) return false;
        if (!buildingUnit.IsUnderConstruct) return false;

        buildingUnit.AssignWorkerUnit(this);
        return true;
    }
}