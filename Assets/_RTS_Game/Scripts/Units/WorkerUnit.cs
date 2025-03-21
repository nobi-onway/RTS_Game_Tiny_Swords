using System;
using UnityEngine;

public class WorkerUnit : HumanoidUnit
{
    [SerializeField] private float m_woodGatherTickTime = 1.0f;
    [SerializeField] private int m_woodPerTick = 1;

    private float m_choppingTimer;
    private int m_woodCollected;
    private int m_goldCollected;
    [SerializeField] private int m_woodCapacity = 10;
    [SerializeField] private int m_goldCapacity = 10;

    [SerializeField] private SpriteRenderer m_holdingWoodSprite;
    [SerializeField] private SpriteRenderer m_holdingGoldSprite;

    private BuildingUnit m_targetBuilding => target?.GetComponent<BuildingUnit>();
    private Tree m_targetTree;
    [SerializeField]
    private EnumSystem<EWorkerTask> m_taskSystem = new();

    public EWorkerTask CurrentTask => m_taskSystem.Value;

    public bool IsHoldingWood => m_woodCollected > 0;
    public bool IsHoldingGold => m_goldCollected > 0;
    public bool IsHoldingResource => IsHoldingWood || IsHoldingGold;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_stateSystem.OnValueChange += HandleOnStateChange;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        m_stateSystem.OnValueChange -= HandleOnStateChange;
    }

    protected override void UpdateBehavior()
    {
        switch (CurrentTask)
        {
            case EWorkerTask.BUILD:
                HandleBuildTask();
                break;
            case EWorkerTask.RETURN_CHOPPING:
                HandleReturnChoppingTask();
                break;
            case EWorkerTask.CHOP:
                HandleChopTask();
                break;
            case EWorkerTask.RETURN_RESOURCE:
                HandleReturnResourceTask();
                break;
            case EWorkerTask.NONE:
                break;
        }

        HandleResourceDisplay();
    }
    protected override void ResetAction()
    {
        m_targetBuilding?.UnassignWorkerUnit(this);

        m_choppingTimer = 0;

        ReleaseFromChop();
    }
    private void HandleOnStateChange(EUnitState newState)
    {
        animator.SetBool("IsBuilding", newState == EUnitState.BUILDING);
        animator.SetBool("IsChopping", newState == EUnitState.CHOPPING);
    }
    private void DetectBuilding()
    {
        if (!IsCloseObject(out BuildingUnit buildingUnit)) return;
        if (buildingUnit != m_targetBuilding) return;

        m_mover.StopMove();

        m_stateSystem.SetValue(EUnitState.IDLE, EUnitState.BUILDING);
    }

    private void HandleBuildTask()
    {
        if (m_stateSystem.IsCurrentValue(EUnitState.BUILDING))
        {
            m_targetBuilding?.UpdateBuildingProgress(Time.fixedDeltaTime);
            return;
        }

        if (m_targetBuilding == null) return;
        DetectBuilding();
    }

    private void HandleChopTask()
    {
        if (m_stateSystem.IsCurrentValue(EUnitState.CHOPPING))
        {
            m_choppingTimer += Time.deltaTime;

            if (m_choppingTimer >= m_woodGatherTickTime)
            {
                m_woodCollected += m_woodPerTick;
                m_choppingTimer = 0;
            }

            if (m_woodCollected == m_woodCapacity)
            {
                ReleaseFromChop();

                m_taskSystem.SetValue(EWorkerTask.RETURN_RESOURCE);
            }

            return;
        }
    }

    private void HandleReturnResourceTask()
    {
        StructureUnit storage = GameManager.Instance.FindClosestStructureUnit(this.transform.position, float.MaxValue);

        if (storage == null) return;

        Vector3 closestPointToStorage = storage.Collider.ClosestPoint(this.transform.position);

        MoveTo(closestPointToStorage, ReturningWood);
    }

    private void HandleReturnChoppingTask()
    {
        Tree closestTree = GameManager.Instance.FindClosestUnClaimedTree(this.transform.position);

        if (closestTree == null) return;

        closestTree.StartExploitBy(this);
    }

    private void HandleResourceDisplay()
    {
        m_holdingGoldSprite.gameObject.SetActive(IsHoldingGold);
        m_holdingWoodSprite.gameObject.SetActive(IsHoldingWood);

        animator.SetFloat(AnimatorParameter.HOLD_RESOURCE_F, IsHoldingResource ? 1 : 0);
    }

    private void ReturningWood()
    {
        m_woodCollected = 0;

        m_taskSystem.SetValue(EWorkerTask.RETURN_CHOPPING);
    }

    //ANIMATION EVENT
    private void HitTargetTree()
    {
        if (!m_targetTree) return;

        Vector2 direction = m_targetTree.transform.position - transform.position;

        spriteRenderer.flipX = direction.x < 0;

        m_targetTree?.TakeHitFrom(direction);
    }

    protected override void HandleOnScanned(Unit unit)
    {
    }

    private void StartChopping()
    {
        m_mover.StopMove();

        m_stateSystem.SetValue(EUnitState.IDLE, EUnitState.CHOPPING);
    }

    protected override void HandleOnDead()
    {
        ResetAction();

        base.HandleOnDead();
    }

    public void AssignToBuildProcess(BuildingUnit targetBuilding)
    {
        if (targetBuilding == null) return;

        SetTarget(targetBuilding);
        MoveTo(targetBuilding.transform.position + Vector3.up * 1.5f, null);

        m_taskSystem.SetValue(EWorkerTask.BUILD);

        UIManager.Instance.DisplayInteractEffect(GeneralUtils.GetTopPosition(targetBuilding.transform), EInteractType.BUILD);
    }

    public void UnassignFromBuildProcess()
    {
        SetTarget(null);
        m_taskSystem.SetValue(EWorkerTask.NONE);
        m_stateSystem.SetValue(EUnitState.BUILDING, EUnitState.IDLE);
    }

    public void SendToChop(Tree tree)
    {
        MoveTo(GeneralUtils.GetBottomPosition(tree.transform), StartChopping);

        m_taskSystem.SetValue(EWorkerTask.CHOP);
        m_targetTree = tree;

        UIManager.Instance.DisplayInteractEffect(tree.transform.position, EInteractType.CHOP);
    }

    private void ReleaseFromChop()
    {
        m_targetTree?.Unexploited();
        m_targetTree = null;

        m_taskSystem.SetValue(EWorkerTask.NONE);
        m_stateSystem.SetValue(EUnitState.CHOPPING, EUnitState.IDLE);
    }

    public override bool TryInteractWithOtherUnit(Unit unit)
    {
        if (!base.TryInteractWithOtherUnit(unit)) return false;

        if (unit is not BuildingUnit buildingUnit) return false;
        if (!buildingUnit.IsUnderConstruct) return false;

        buildingUnit.AssignWorkerUnit(this);

        return false;
    }
}