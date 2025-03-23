using System;
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
            m_stanceSystem.SetValue(stanceActionSO.CombatStance);
        }
    }

    protected override void UpdateBehavior()
    {
        if (m_stateSystem.IsCurrentValue(EUnitState.DEAD)) return;

        if (m_taskSystem.IsCurrentValue(ECombatTask.FIGHT))
        {
            if (m_stanceSystem.IsCurrentValue(ECombatStance.OFFENSIVE)) DoChase();

            if (m_stateSystem.IsCurrentValue(EUnitState.ATTACKING)) DoAttack();

            m_stateSystem.SetValue(m_attack.IsInAttackRange(target) ? EUnitState.ATTACKING : EUnitState.IDLE);
        }
    }

    protected override void HandleDestinationReached()
    {
        m_taskSystem.SetValue(ECombatTask.GUARD);
    }

    protected virtual void DoAttack()
    {
        m_attack.TryToAttack(target);

        bool canDamageUnit = m_attack.CanDamageUnit(target, out HealthController healthController);

        if (!canDamageUnit) m_taskSystem.SetValue(ECombatTask.FIGHT, ECombatTask.GUARD);
    }

    private void DoChase()
    {
        bool isInAttackRange = m_attack.IsInAttackRange(target);

        if (!isInAttackRange) { m_mover.MoveTo(target.transform.position, null); return; }

        m_mover.StopMove();
    }

    protected override void HandleOnScanned(Unit unit)
    {
        if (CurrentTask != ECombatTask.GUARD) return;

        base.HandleOnScanned(unit);

        bool canFight = hasTarget
                        && (CurrentStance == ECombatStance.OFFENSIVE
                            || (CurrentStance == ECombatStance.DEFENSIVE && m_attack.IsInAttackRange(target)));

        if (canFight) m_taskSystem.SetValue(ECombatTask.GUARD, ECombatTask.FIGHT);
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