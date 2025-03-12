using UnityEngine;

public class WarriorUnit : HumanoidUnit
{
    private MeleeAttack m_meleeAttack;
    private SelectableUnit m_selectableUnit;

    [SerializeField]
    private EnumSystem<EWarriorTask> m_taskSystem = new();
    public EWarriorTask CurrentTask => m_taskSystem.Value;

    [SerializeField]
    private EnumSystem<EWarriorStance> m_stanceSystem = new();
    public EWarriorStance CurrentStance => m_stanceSystem.Value;
    public EnumSystem<EWarriorStance> StanceSystem => m_stanceSystem;

    protected override void Awake()
    {
        base.Awake();

        GeneralUtils.SetUpComponent<MeleeAttack>(this.transform, ref m_meleeAttack);
        GeneralUtils.SetUpComponent<SelectableUnit>(this.transform, ref m_selectableUnit);
    }

    protected override void Start()
    {
        base.Start();

        m_taskSystem.SetValue(EWarriorTask.GUARD);

        if (m_selectableUnit.GetCurrentActionSO() is StanceActionSO stanceActionSO)
        {
            m_stanceSystem.SetValue(stanceActionSO.WarriorStance);
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        m_mover.OnDestinationReached += HandleOnDestinationReached;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        m_mover.OnDestinationReached -= HandleOnDestinationReached;
    }

    protected override void UpdateBehavior()
    {
        base.UpdateBehavior();

        if (m_taskSystem.IsCurrentValue(EWarriorTask.FIGHT))
        {
            DoAttack();
        }
    }

    private void HandleOnDestinationReached()
    {
        m_taskSystem.SetValue(EWarriorTask.GUARD);
    }

    private void DoAttack()
    {
        bool isInAttackRange = m_meleeAttack.IsInAttackRange(target);

        if (!isInAttackRange) { m_mover.MoveTo(target.transform.position); return; }

        m_mover.StopMove();

        bool isUnderAttacking = m_meleeAttack.TryToAttack(target);

        EUnitState unitState = isUnderAttacking ? EUnitState.ATTACKING : EUnitState.IDLE;

        m_stateSystem.SetValue(unitState);
    }

    protected override void HandleOnScanned(Unit unit)
    {
        if (CurrentStance == EWarriorStance.DEFENSIVE) return;

        if (CurrentTask != EWarriorTask.GUARD) return;

        base.HandleOnScanned(unit);

        if (hasTarget) m_taskSystem.SetValue(EWarriorTask.GUARD, EWarriorTask.FIGHT);
    }

    protected override void ResetAction()
    {
        base.ResetAction();

        m_taskSystem.SetValue(EWarriorTask.NONE);
        SetTarget(null);
    }

    public override bool TryInteractWithOtherUnit(Unit unit)
    {
        if (!base.TryInteractWithOtherUnit(unit)) return false;

        if (unit is not EnemyUnit enemyUnit) return false;

        SetTarget(enemyUnit);
        m_taskSystem.SetValue(EWarriorTask.FIGHT);

        UIManager.Instance.DisplayInteractEffect(GeneralUtils.GetTopPosition(enemyUnit.transform), EInteractType.FIGHT);
        return true;
    }
}