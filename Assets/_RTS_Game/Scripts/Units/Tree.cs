using UnityEngine;

public class Tree : MonoBehaviour
{
    [SerializeField] private CapsuleCollider2D m_capsuleCollider2D;
    private Animator m_animator;

    private bool m_exploited = false;
    public bool IsExploited => m_exploited;

    private void Awake()
    {
        GeneralUtils.SetUpComponent<Animator>(transform, ref m_animator);
    }

    public void StartExploitBy(WorkerUnit workerUnit)
    {
        TryToExploit(workerUnit);
    }

    public void Unexploited()
    {
        m_exploited = false;
    }

    public void TakeHitFrom(Vector2 direction)
    {
        m_animator.SetTrigger(direction.x < 0 ? AnimatorParameter.HIT_RIGHT_TRIG : AnimatorParameter.HIT_LEFT_TRIG);
    }

    private bool TryToExploit(WorkerUnit workerUnit)
    {
        if (m_exploited) return false;

        workerUnit.SendToChop(this);
        m_exploited = true;

        return true;
    }
}