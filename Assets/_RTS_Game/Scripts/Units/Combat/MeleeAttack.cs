using System;
using System.Collections;
using UnityEngine;

public class MeleeAttack : MonoBehaviour, IActionNode
{
    private const string HORIZONTAL_ATK = "Horizontal_Attack";
    private const string UP_ATK = "Up_Attack";
    private const string DOWN_ATK = "Down_Attack";

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

    private bool TryToAttack(Unit unit)
    {
        if (m_currentCoolDownTime < m_attackCoolDownTime) return false;
        if (!unit) return false;

        PerformAttackAnimation(unit.transform.position);

        StartCoroutine(IE_DelayDamage(m_attackDamageDelay, m_damage, unit));

        m_currentCoolDownTime = 0;
        return true;
    }

    private void TakeDamage(int damage, Unit target)
    {
        Debug.Log($"{target} was taken {damage} damage from {this.gameObject.name}");

        UIManager.Instance.SpawnTextPopup(target.GetTopPosition(), m_damage.ToString(), Color.red);
    }

    private IEnumerator IE_DelayDamage(float delay, int damage, Unit target)
    {
        yield return new WaitForSeconds(delay);

        TakeDamage(damage, target);
    }

    private void PerformAttackAnimation(Vector3 targetPosition)
    {
        Vector2 atkDirection = (targetPosition - this.transform.position).normalized;

        if (Mathf.Abs(atkDirection.x) > Mathf.Abs(atkDirection.y))
        {
            m_animator.SetTrigger(HORIZONTAL_ATK);
        }
        else
        {
            m_animator.SetTrigger(atkDirection.y < 0 ? DOWN_ATK : UP_ATK);
        }
    }

    public EStatusNode Execute(Blackboard blackboard, Action onSuccess)
    {
        Unit target = blackboard.Get<Unit>(Blackboard.CLASS_TARGET);

        bool isInAtkRange = Vector2.Distance(target.transform.position, this.transform.position) <= m_attackRange;

        if (!isInAtkRange) return EStatusNode.FAILURE;

        TryToAttack(target);
        onSuccess?.Invoke();
        return EStatusNode.SUCCESS;
    }
}