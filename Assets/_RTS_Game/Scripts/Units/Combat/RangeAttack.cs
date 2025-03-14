using System;
using System.Collections;
using UnityEngine;

public class RangeAttack : Attack
{
    [SerializeField] private Projectile m_projectilePrefab;

    private Unit m_unit;

    private void Awake()
    {
        GeneralUtils.SetUpComponent<Unit>(transform, ref m_unit);
    }

    public override EStatusNode Execute(Blackboard blackboard, Action onSuccess = null)
    {
        return EStatusNode.SUCCESS;
    }

    public override bool TryToAttack(Unit unit)
    {
        if (!TryToAttack(unit, out HealthController healthController)) return false;

        StartCoroutine(IE_DelayAttack(0.6f, unit));
        return true;
    }

    private IEnumerator IE_DelayAttack(float delay, Unit target)
    {
        yield return new WaitForSeconds(delay);

        Projectile projectileClone = Instantiate(m_projectilePrefab, this.transform.position, Quaternion.identity);

        projectileClone.Initialize(m_unit, target, m_damage);
    }

    protected override void PerformAttackAnimation(Vector3 targetPosition)
    {
        base.PerformAttackAnimation(targetPosition);

        Vector2 atkDirection = (targetPosition - this.transform.position).normalized;

        bool isHorizontal = Mathf.Abs(atkDirection.x) > Mathf.Abs(atkDirection.y);
        bool isVertical = !isHorizontal;
        bool isUp = atkDirection.y > 0;

        if (isHorizontal)
        {
            float angle = Mathf.Atan2(Mathf.Abs(atkDirection.y), Mathf.Abs(atkDirection.x)) * Mathf.Rad2Deg;

            if (angle < 15) { m_animator.SetTrigger(AnimatorParameter.HORIZONTAL_ATK_TRIG); return; }
        }

        if (isVertical)
        {
            float angle = Mathf.Atan2(Mathf.Abs(atkDirection.x), Mathf.Abs(atkDirection.y)) * Mathf.Rad2Deg;

            if (angle < 15) { m_animator.SetTrigger(isUp ? AnimatorParameter.UP_ATK_TRIG : AnimatorParameter.DOWN_ATK_TRIG); return; }
        }

        m_animator.SetTrigger(isUp ? AnimatorParameter.UP_DIAGONAL_ATK_TRIG : AnimatorParameter.DOWN_DIAGONAL_ATK_TRIG);
    }
}