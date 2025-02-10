using UnityEngine;

public class HumanoidUnit : Unit
{
    private Vector2 m_velocity;
    private Vector2 m_lastPos;

    private float m_currentSpeed => m_velocity.magnitude;

    private void Start()
    {
        m_lastPos = transform.position;
    }

    private void FixedUpdate()
    {
        m_velocity = new Vector2(
            transform.position.x - m_lastPos.x,
            transform.position.y - m_lastPos.y
        ) / Time.deltaTime;

        m_lastPos = transform.position;

        animator.SetFloat("Speed", Mathf.Clamp01(m_currentSpeed));
    }
}