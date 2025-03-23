using UnityEngine;
using UnityEngine.Events;

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
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    private void FixedUpdate()
    {
        UpdateBehavior();
    }

    protected void MoveTo(Vector3 position, UnityAction onArrived)
    {
        m_mover.MoveTo(position, () => { m_stateSystem.SetValue(EUnitState.IDLE); onArrived?.Invoke(); });
        m_stateSystem.SetValue(EUnitState.MOVING);
    }

    protected override void HandleOnSetTarget(Unit target)
    {
        base.HandleOnSetTarget(target);
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

        MoveTo(position, HandleDestinationReached);
        UIManager.Instance.DisplayClickEffect(position);
    }

    protected virtual void HandleDestinationReached() { }
    protected virtual void ResetAction() { }
    protected virtual void UpdateBehavior() { }
}