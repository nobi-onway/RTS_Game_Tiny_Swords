using UnityEngine;

public class EnemyUnit : HumanoidUnit
{
    public override EUnitClass Class => EUnitClass.ENEMY;
    private MeleeAttack m_meleeAttack;
    private BehaviorNode root;

    protected override void Awake()
    {
        base.Awake();

        GeneralUtils.SetUpComponent<MeleeAttack>(this.transform, ref m_meleeAttack);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        m_unitRadar.OnScanned += SetTarget;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        m_unitRadar.OnScanned -= SetTarget;
    }

    protected void Start()
    {
        Blackboard blackboard = new Blackboard();

        root = new SequenceNode(
            new ActionNode(m_unitRadar, blackboard),
            new SelectorNode(
                new ActionNode(m_meleeAttack, blackboard, () => { m_mover.StopMove(); m_stateSystem.SetValue(EUnitState.ATTACKING); }),
                new ActionNode(m_mover, blackboard)
            )
        );
    }

    protected override void UpdateBehavior()
    {
        if (root != null) root.Execute();
    }
}