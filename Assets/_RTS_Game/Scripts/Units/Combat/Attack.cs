using System;
using UnityEngine;
public abstract class Attack : MonoBehaviour, IActionNode
{
    [SerializeField] protected int m_damage;
    [SerializeField] protected float m_attackRange;
    [SerializeField] protected float m_attackCoolDownTime;
    protected float m_currentCoolDownTime = 0;
    protected Animator m_animator;
    protected SpriteRenderer m_spriteRenderer;

    public abstract EStatusNode Execute(Blackboard blackboard, Action onSuccess = null);

    protected void Start()
    {
        GeneralUtils.SetUpComponent<Animator>(this.transform, ref m_animator);
        GeneralUtils.SetUpComponent<SpriteRenderer>(this.transform, ref m_spriteRenderer);
    }

    protected void FixedUpdate()
    {
        if (m_currentCoolDownTime < m_attackCoolDownTime) m_currentCoolDownTime += Time.deltaTime;
    }

    protected bool TryToAttack(Unit unit, out HealthController healthController)
    {
        healthController = null;

        bool isInCoolDownTime = m_currentCoolDownTime < m_attackCoolDownTime;
        if (isInCoolDownTime) return false;

        bool isTargetDead = unit.CurrentState == EUnitState.DEAD;
        if (isTargetDead) return false;

        bool hasHealth = unit.TryGetComponent(out healthController);
        if (!hasHealth) return false;

        PerformAttackAnimation(unit.transform.position);

        m_currentCoolDownTime = 0;
        return true;
    }

    public bool IsInAttackRange(Unit target)
    {
        Collider2D targetCollider = target.Collider;
        Vector3 closestPoint = targetCollider.ClosestPoint(this.transform.position);

        return Vector2.Distance(closestPoint, this.transform.position) <= m_attackRange;
    }

    public abstract bool TryToAttack(Unit unit);
    protected virtual void PerformAttackAnimation(Vector3 targetPosition)
    {
        m_spriteRenderer.flipX = (targetPosition - this.transform.position).normalized.x < 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.25f);
        Gizmos.DrawSphere(this.transform.position, m_attackRange);
    }
}