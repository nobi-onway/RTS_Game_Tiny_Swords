using System;
using System.Collections;
using UnityEngine;

public class RangeAttack : Attack
{
    [SerializeField] private Projectile m_projectilePrefab;
    public Func<Vector3> CalculateFirePosition;

    [SerializeField] private EAttackTrig[] m_attackTrig = (EAttackTrig[])Enum.GetValues(typeof(EAttackTrig));

    private Unit m_unit;

    private void Awake()
    {
        GeneralUtils.SetUpComponent<Unit>(transform, ref m_unit);
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

        Projectile projectileClone = Instantiate(m_projectilePrefab, GetFirePosition(), Quaternion.identity);

        projectileClone.Initialize(m_unit, target, m_damage);
    }

    private Vector3 GetFirePosition()
    {
        if (CalculateFirePosition == null) return this.transform.position;

        return CalculateFirePosition();
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

            if (angle < 15) { m_animator.SetTrigger(m_attackTrig[0].ToString()); return; }
        }

        if (isVertical)
        {
            float angle = Mathf.Atan2(Mathf.Abs(atkDirection.x), Mathf.Abs(atkDirection.y)) * Mathf.Rad2Deg;

            if (angle < 15) { m_animator.SetTrigger(isUp ? m_attackTrig[1].ToString() : m_attackTrig[2].ToString()); return; }
        }

        m_animator.SetTrigger(isUp ? m_attackTrig[3].ToString() : m_attackTrig[4].ToString());
    }
}