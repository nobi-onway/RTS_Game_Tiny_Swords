using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float m_speed = 10.0f;
    private int m_damage;

    private Unit m_target;
    private Unit m_owner;

    public void Initialize(Unit owner, Unit target, int damage)
    {
        this.m_owner = owner;
        this.m_target = target;
        this.m_damage = damage;
    }

    private void Update()
    {
        if (m_target == null || m_target.CurrentState == EUnitState.DEAD)
        {
            Destroy(gameObject);
            return;
        }

        UpdatePosition();
    }

    private void UpdatePosition()
    {
        Vector3 direction = (m_target.transform.position - this.transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        this.transform.position += direction * m_speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Unit unit = other.GetComponent<Unit>();

        if (unit != m_target) return;

        if (!unit.TryGetComponent(out HealthController healthController)) return;

        healthController.TakeDamage(m_damage);

        Destroy(gameObject);
    }
}