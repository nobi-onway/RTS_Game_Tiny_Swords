using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AIPawn : MonoBehaviour
{
    [SerializeField]
    private float m_speed;
    private List<Vector3> m_currentPath = new();
    private int m_currentNodeIndex;
    private SpriteRenderer m_spriteRenderer;

    private void Start()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
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

    public void SetDestination(Vector3 destination)
    {
        PathFinding pathFinding = TilemapManager.Instance.PathFinding;

        if (m_currentPath != null && m_currentPath.Count > 0)
        {
            PathNode newEndNode = pathFinding.FindNode(destination);
            if (newEndNode == null) return;

            Vector3 endDestination = new Vector3(newEndNode.centerX, newEndNode.centerY);
            if (Vector3.Distance(endDestination, m_currentPath[^1]) < 0.1f) return;
        }

        m_currentPath = pathFinding.FindPath(this.transform.position, destination);
        m_currentNodeIndex = 0;
    }

    private bool IsPathValid()
    {
        return m_currentPath != null && m_currentNodeIndex < m_currentPath.Count;
    }
}