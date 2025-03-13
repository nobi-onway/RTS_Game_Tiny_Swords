using UnityEngine;

public class HumanoidUnit : Unit
{
    protected Mover m_mover;

    protected override void Awake()
    {
        base.Awake();

        GeneralUtils.SetUpComponent<Mover>(this.transform, ref m_mover);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        m_mover.OnMove += ToMoveStateIf;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        m_mover.OnMove -= ToMoveStateIf;
    }

    private void FixedUpdate()
    {
        UpdateBehavior();
    }

    protected void MoveTo(Vector3 position)
    {
        m_mover.MoveTo(position);
        m_stateSystem.SetValue(EUnitState.MOVING);
    }

    protected override void HandleOnSetTarget(Unit target)
    {
        base.HandleOnSetTarget(target);

        // if (target == null) m_mover.StopMove();
    }

    protected override void HandleOnDead()
    {
        base.HandleOnDead();

        m_mover.StopMove();
    }

    public override void DoActionAt(Vector2 position)
    {
        if (!CanPerformAction()) return;

        ResetAction();

        MoveTo(position);
        UIManager.Instance.DisplayClickEffect(position);
    }
    protected virtual void ResetAction() { }
    protected virtual void UpdateBehavior() { }
    private void ToMoveStateIf(bool isMoving)
    {
        if (!isMoving) m_stateSystem.SetValue(EUnitState.MOVING, EUnitState.IDLE);
        if (isMoving) m_stateSystem.SetValue(EUnitState.MOVING);
    }
}