using System;
using UnityEngine;
using UnityEngine.Events;

public class Mover : MonoBehaviour, IActionNode
{
    private AIPawn m_AIPawn;
    private Animator m_animator;
    private Vector3 m_lastPosition;
    private Vector2 m_velocity;
    private float m_currentSpeed => m_velocity.magnitude;
    private float m_smoothSpeed;
    private float m_smoothFactor = 50;
    private bool m_isMoving;

    public event Action<bool> OnMove;
    public UnityAction OnDestinationReached;

    private void Awake()
    {
        GeneralUtils.SetUpComponent<AIPawn>(this.transform, ref m_AIPawn);
        GeneralUtils.SetUpComponent<Animator>(this.transform, ref m_animator);
    }

    private void OnEnable()
    {
        m_AIPawn.OnDestinationReached += OnDestinationReached;
    }

    private void OnDisable()
    {
        m_AIPawn.OnDestinationReached -= OnDestinationReached;
    }

    private void Start()
    {
        m_lastPosition = this.transform.position;
    }

    private void FixedUpdate()
    {
        UpdateVelocity(this.transform.position);
    }

    public void MoveTo(Vector3 position)
    {
        m_AIPawn.SetDestination(position);
    }

    public void StopMove()
    {
        m_AIPawn.SetDestination(null);
    }

    private void UpdateVelocity(Vector3 currentPosition)
    {
        m_velocity = new Vector2(
                    currentPosition.x - m_lastPosition.x,
                    currentPosition.y - m_lastPosition.y
                ) / Time.deltaTime;

        m_lastPosition = currentPosition;
        m_smoothSpeed = Mathf.Lerp(m_smoothSpeed, m_currentSpeed, m_smoothFactor * Time.deltaTime);

        m_isMoving = m_smoothSpeed > 0;

        OnMove?.Invoke(m_isMoving);

        m_animator.SetFloat(AnimatorParameter.SPEED_F, Mathf.Clamp01(m_smoothSpeed));
    }

    public EStatusNode Execute(Blackboard blackboard, Action onSuccess)
    {
        Unit target = blackboard.Get<Unit>(Blackboard.CLASS_TARGET);
        MoveTo(target.transform.position);

        return m_isMoving ? EStatusNode.RUNNING : EStatusNode.SUCCESS;
    }
}