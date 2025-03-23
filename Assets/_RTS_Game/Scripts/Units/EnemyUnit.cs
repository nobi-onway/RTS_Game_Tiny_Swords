public class EnemyUnit : HumanoidUnit
{
    public override EUnitClass Class => EUnitClass.ENEMY;
    private Attack m_attack;
    private BehaviorNode root;

    protected override void Awake()
    {
        base.Awake();

        GeneralUtils.SetUpComponent<Attack>(this.transform, ref m_attack);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    protected override void Start()
    {
        base.Start();

        Blackboard blackboard = new Blackboard();

        root = new SequenceNode(
            new ActionNode(m_unitRadar, blackboard),
            new SelectorNode(
                new ActionNode(m_attack, blackboard, DoAttack),
                new ActionNode(m_mover, blackboard)
            )
        );
    }

    private void DoAttack()
    {
        m_mover.StopMove();

        bool isUnderAttacking = m_attack.TryToAttack(target);

        if (isUnderAttacking)
        {
            spriteRenderer.flipX = (target.transform.position - this.transform.position).normalized.x < 0;
        }

        EUnitState unitState = isUnderAttacking ? EUnitState.ATTACKING : EUnitState.IDLE;

        m_stateSystem.SetValue(unitState);
    }

    protected override void UpdateBehavior()
    {
        if (m_stateSystem.IsCurrentValue(EUnitState.DEAD)) return;

        root?.Execute();
    }
}