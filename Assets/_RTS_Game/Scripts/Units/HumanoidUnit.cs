using UnityEngine;

public class HumanoidUnit : Unit
{
    protected Mover m_mover;
    protected UnitRadar m_unitRadar;

    protected override void Awake()
    {
        base.Awake();

        GeneralUtils.SetUpComponent<Mover>(this.transform, ref m_mover);
        GeneralUtils.SetUpComponent<UnitRadar>(this.transform, ref m_unitRadar);
    }

    protected virtual void OnEnable()
    {
        m_mover.OnMove += ToMoveStateIf;
    }

    protected virtual void OnDisable()
    {
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

    public override void DoActionAt(Vector2 position)
    {
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