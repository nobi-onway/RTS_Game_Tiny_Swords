using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MeleeAttack : Attack
{
    [SerializeField] private float m_attackDamageDelay = 0.5f;

    public override bool TryToAttack(Unit unit)
    {
        if (!TryToAttack(unit, out HealthController healthController)) return false;

        StartCoroutine(IE_DelayDamage(m_attackDamageDelay, m_damage, healthController));

        return true;
    }

    private void DealDamage(int damage, HealthController targetHealth)
    {
        targetHealth.TakeDamage(damage);
    }

    private IEnumerator IE_DelayDamage(float delay, int damage, HealthController targetHealth)
    {
        yield return new WaitForSeconds(delay);

        DealDamage(damage, targetHealth);
    }

    protected override void PerformAttackAnimation(Vector3 targetPosition)
    {
        base.PerformAttackAnimation(targetPosition);

        Vector2 atkDirection = (targetPosition - this.transform.position).normalized;

        if (Mathf.Abs(atkDirection.x) > Mathf.Abs(atkDirection.y))
        {
            m_animator.SetTrigger(AnimatorParameter.HORIZONTAL_ATK_TRIG);
        }
        else
        {
            m_animator.SetTrigger(atkDirection.y < 0 ? AnimatorParameter.DOWN_ATK_TRIG : AnimatorParameter.UP_ATK_TRIG);
        }
    }

    public override EStatusNode Execute(Blackboard blackboard, Action onSuccess)
    {
        Unit target = blackboard.Get<Unit>(Blackboard.CLASS_TARGET);

        bool isInAtkRange = IsInAttackRange(target);

        if (!isInAtkRange) return EStatusNode.FAILURE;

        onSuccess();
        return EStatusNode.SUCCESS;
    }
}