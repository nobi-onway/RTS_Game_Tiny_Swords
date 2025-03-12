using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MeleeAttack : MonoBehaviour, IActionNode
{
    [SerializeField] private int m_damage;
    [SerializeField] private float m_attackRange;
    [SerializeField] private float m_attackCoolDownTime;
    [SerializeField] private float m_attackDamageDelay = 0.5f;
    private float m_currentCoolDownTime;
    private Animator m_animator;

    private void Start()
    {
        GeneralUtils.SetUpComponent<Animator>(this.transform, ref m_animator);
    }

    private void FixedUpdate()
    {
        if (m_currentCoolDownTime < m_attackCoolDownTime) m_currentCoolDownTime += Time.deltaTime;
    }

    public bool TryToAttack(Unit unit)
    {
        bool isInCoolDownTime = m_currentCoolDownTime < m_attackCoolDownTime;
        if (isInCoolDownTime) return false;

        bool isTargetDead = unit.CurrentState == EUnitState.DEAD;
        if (isTargetDead) return false;

        bool hasHealth = unit.TryGetComponent(out HealthController healthController);
        if (!hasHealth) return false;

        PerformAttackAnimation(unit.transform.position);

        StartCoroutine(IE_DelayDamage(m_attackDamageDelay, m_damage, healthController));

        m_currentCoolDownTime = 0;
        return true;
    }

    public bool IsInAttackRange(Unit unit) => Vector2.Distance(unit.transform.position, this.transform.position) <= m_attackRange;

    private void DealDamage(int damage, HealthController targetHealth)
    {
        targetHealth.TakeDamage(damage);
    }

    private IEnumerator IE_DelayDamage(float delay, int damage, HealthController targetHealth)
    {
        yield return new WaitForSeconds(delay);

        DealDamage(damage, targetHealth);
    }

    private void PerformAttackAnimation(Vector3 targetPosition)
    {
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

    public EStatusNode Execute(Blackboard blackboard, Action onSuccess)
    {
        Unit target = blackboard.Get<Unit>(Blackboard.CLASS_TARGET);

        bool isInAtkRange = IsInAttackRange(target);

        if (!isInAtkRange) return EStatusNode.FAILURE;

        onSuccess();
        return EStatusNode.SUCCESS;
    }
}