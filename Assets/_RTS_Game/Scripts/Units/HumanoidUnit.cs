using UnityEngine;

public abstract class HumanoidUnit : Unit
{
    private Vector2 m_velocity;
    private Vector2 m_lastPos;
    protected AIPawn AIPawn;
    private float m_currentSpeed => m_velocity.magnitude;

    private bool m_isMoving;

    protected override void Awake()
    {
        base.Awake();
        SetUpComponent<AIPawn>(ref AIPawn);
    }

    private void Start()
    {
        m_lastPos = transform.position;
    }

    private void FixedUpdate()
    {
        UpdateVelocity();
        UpdateBehavior();
    }

    public void MoveTo(Vector3 position)
    {
        Vector2 direction = (position - this.transform.position).normalized;
        spriteRenderer.flipX = direction.x < 0;

        AIPawn.SetDestination(position);
        OnSetDestination();
    }

    private void UpdateVelocity()
    {
        m_velocity = new Vector2(
                    transform.position.x - m_lastPos.x,
                    transform.position.y - m_lastPos.y
                ) / Time.deltaTime;

        m_lastPos = transform.position;
        m_isMoving = m_currentSpeed > 0;

        if (!m_isMoving) OnStopMove();

        animator.SetFloat("Speed", Mathf.Clamp01(m_currentSpeed));
    }

    protected abstract void UpdateBehavior();
    protected virtual void OnSetDestination() { }
    protected virtual void OnStopMove() { }
}