using UnityEngine;

public enum EWorkerTask { None, Build }
public enum EWorkerState { Idle, Moving, Building }

public class WorkerUnit : HumanoidUnit
{
    [SerializeField]
    private BuildingUnit m_targetBuilding;

    private EnumSystem<EWorkerState> m_stateSystem = new EnumSystem<EWorkerState>();
    private EnumSystem<EWorkerTask> m_taskSystem = new EnumSystem<EWorkerTask>();

    public EWorkerState CurrentState => m_stateSystem.Value;
    public EWorkerTask CurrentTask => m_taskSystem.Value;

    private void OnEnable()
    {
        m_stateSystem.OnValueChange += OnStateChange;
    }

    private void OnDisable()
    {
        m_stateSystem.OnValueChange -= OnStateChange;
    }

    protected override void UpdateBehavior()
    {
        if (m_targetBuilding == null) return;

        DetectBuilding();

        if (m_stateSystem.IsCurrentValue(EWorkerState.Building))
        {
            m_targetBuilding?.UpdateBuildingProgress(Time.fixedDeltaTime);
        }
    }

    protected override void OnSetDestination()
    {
        m_stateSystem.SetValue(EWorkerState.Moving);
        m_targetBuilding?.UnassignWorkerUnit(this);
    }

    protected override void OnStopMove()
    {
        m_stateSystem.SetValue(EWorkerState.Moving, EWorkerState.Idle);
    }

    private void OnStateChange(EWorkerState newState)
    {
        animator.SetBool("IsBuilding", newState == EWorkerState.Building);
    }
    private void DetectBuilding()
    {
        if (!IsCloseObject(out BuildingUnit buildingUnit)) return;
        if (buildingUnit != m_targetBuilding) return;

        m_stateSystem.SetValue(EWorkerState.Idle, EWorkerState.Building);
    }

    public void AssignToBuildProcess(BuildingUnit targetBuilding)
    {
        if (targetBuilding == null) return;

        MoveTo(targetBuilding.transform.position + Vector3.up * 1.5f);

        m_targetBuilding = targetBuilding;
        m_taskSystem.SetValue(EWorkerTask.Build);
    }

    public void UnassignFromBuildProcess()
    {
        m_targetBuilding = null;
        m_taskSystem.SetValue(EWorkerTask.None);
        m_stateSystem.SetValue(EWorkerState.Building, EWorkerState.Idle);
    }

    public override bool TryInteractWithOtherUnit(Unit unit)
    {
        if (unit is not BuildingUnit buildingUnit) return false;
        if (!buildingUnit.IsUnderConstruct) return false;

        buildingUnit.AssignWorkerUnit(this);
        return true;
    }
}