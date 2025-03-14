using UnityEngine;

public class CombatUnit : HumanoidUnit
{
    protected Attack m_attack;
    private SelectableUnit m_selectableUnit;

    [SerializeField]
    protected EnumSystem<ECombatTask> m_taskSystem = new();
    public ECombatTask CurrentTask => m_taskSystem.Value;

    [SerializeField]
    protected EnumSystem<ECombatStance> m_stanceSystem = new();
    public ECombatStance CurrentStance => m_stanceSystem.Value;
    public EnumSystem<ECombatStance> StanceSystem => m_stanceSystem;

    protected override void Awake()
    {
        base.Awake();

        GeneralUtils.SetUpComponent<Attack>(this.transform, ref m_attack);
        GeneralUtils.SetUpComponent<SelectableUnit>(this.transform, ref m_selectableUnit);
    }

    protected override void Start()
    {
        base.Start();

        m_taskSystem.SetValue(ECombatTask.GUARD);

        if (m_selectableUnit.GetCurrentActionSO() is StanceActionSO stanceActionSO)
        {
            m_stanceSystem.SetValue(stanceActionSO.WarriorStance);
        }

        Debug.Log(this.gameObject.name + " Start");
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

        if (m_taskSystem.IsCurrentValue(ECombatTask.FIGHT))
        {
            DoAttack();
        }
    }

    protected void HandleOnDestinationReached()
    {
        m_taskSystem.SetValue(ECombatTask.GUARD);
    }

    protected virtual void DoAttack()
    {
        bool isInAttackRange = m_attack.IsInAttackRange(target);

        if (!isInAttackRange) { m_mover.MoveTo(target.transform.position); return; }

        m_mover.StopMove();

        bool isUnderAttacking = m_attack.TryToAttack(target);

        EUnitState unitState = isUnderAttacking ? EUnitState.ATTACKING : EUnitState.IDLE;

        m_stateSystem.SetValue(unitState);
    }

    protected override void HandleOnScanned(Unit unit)
    {
        if (CurrentStance == ECombatStance.DEFENSIVE) return;

        if (CurrentTask != ECombatTask.GUARD) return;

        base.HandleOnScanned(unit);

        if (hasTarget) m_taskSystem.SetValue(ECombatTask.GUARD, ECombatTask.FIGHT);
    }

    protected override void ResetAction()
    {
        base.ResetAction();

        m_taskSystem.SetValue(ECombatTask.NONE);
        SetTarget(null);
    }

    public override bool TryInteractWithOtherUnit(Unit unit)
    {
        if (!base.TryInteractWithOtherUnit(unit)) return false;

        if (unit is not EnemyUnit enemyUnit) return false;

        SetTarget(enemyUnit);
        m_taskSystem.SetValue(ECombatTask.FIGHT);

        UIManager.Instance.DisplayInteractEffect(GeneralUtils.GetTopPosition(enemyUnit.transform), EInteractType.FIGHT);
        return true;
    }
}