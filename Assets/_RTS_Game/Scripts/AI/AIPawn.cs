using System.Collections.Generic;
using UnityEngine;

public class AIPawn : MonoBehaviour
{
    [SerializeField]
    private float m_speed;
    [SerializeField]
    private List<Vector3> m_currentPath = new();
    private int m_currentNodeIndex;
    private SpriteRenderer m_spriteRenderer;

    private void Awake()
    {
        GeneralUtils.SetUpComponent<SpriteRenderer>(this.transform, ref m_spriteRenderer);
    }

    private void FixedUpdate()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (!IsPathValid()) return;

        Vector3 targetPosition = m_currentPath[m_currentNodeIndex];

        Vector3 direction = (targetPosition - transform.position).normalized;
        m_spriteRenderer.flipX = direction.x < 0;

        this.transform.position += direction * Time.deltaTime * m_speed;

        if (Vector2.Distance(targetPosition, transform.position) < 0.1f)
        {
            if (m_currentNodeIndex == m_currentPath.Count - 1)
            {
                m_currentPath = null;
            }
            else
            {
                m_currentNodeIndex++;
            }
        }
    }

    public void SetDestination(Vector3? destination)
    {
        if (!destination.HasValue)
        {
            m_currentPath = null;
            m_currentNodeIndex = 0;
            return;
        }

        PathFinding pathFinding = TilemapManager.Instance.PathFinding;

        if (m_currentPath != null && m_currentPath.Count > 0)
        {
            PathNode newEndNode = pathFinding.FindNode(destination.Value);
            if (newEndNode == null) return;

            Vector3 endDestination = new Vector3(newEndNode.centerX, newEndNode.centerY);
            if (Vector3.Distance(endDestination, m_currentPath[^1]) < 0.1f) return;
        }

        m_currentPath = pathFinding.FindPath(this.transform.position, destination.Value);
        m_currentNodeIndex = 0;
    }

    private bool IsPathValid()
    {
        return m_currentPath != null && m_currentNodeIndex < m_currentPath.Count;
    }
}