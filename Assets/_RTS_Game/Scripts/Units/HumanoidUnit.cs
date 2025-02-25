using UnityEngine;

public abstract class HumanoidUnit : Unit
{
    private Vector2 m_velocity;
    private Vector2 m_lastPos;
    protected AIPawn AIPawn;
    private float m_currentSpeed => m_velocity.magnitude;
    private float m_smoothSpeed;
    private float m_smoothFactor = 50;

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
        m_smoothSpeed = Mathf.Lerp(m_smoothSpeed, m_currentSpeed, m_smoothFactor * Time.deltaTime);

        m_isMoving = m_smoothSpeed > 0;

        if (!m_isMoving) OnStopMove();

        animator.SetFloat("Speed", Mathf.Clamp01(m_smoothSpeed));
    }

    protected abstract void UpdateBehavior();
    protected virtual void OnSetDestination() { }
    protected virtual void OnStopMove() { }
}