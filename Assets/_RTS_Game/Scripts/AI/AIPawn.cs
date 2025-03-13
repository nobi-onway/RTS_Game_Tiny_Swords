using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class AIPawn : MonoBehaviour
{
    [SerializeField]
    private float m_speed;
    [SerializeField]
    private List<Vector3> m_currentPath = new();
    private int m_currentNodeIndex;
    private SpriteRenderer m_spriteRenderer;
    public UnityAction OnDestinationReached;

    private Unit m_unit;

    [Header("Separation")]
    [SerializeField] private float m_separationRadius;
    [SerializeField] private float m_separationForce;
    [SerializeField] private bool m_enableSeparation = true;

    private void Awake()
    {
        GeneralUtils.SetUpComponent<SpriteRenderer>(this.transform, ref m_spriteRenderer);
        GeneralUtils.SetUpComponent<Unit>(this.transform, ref m_unit);
    }

    private void FixedUpdate()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (!IsPathValid()) return;

        Vector3 separationDirection = m_enableSeparation ? CalculateSeparation() : Vector3.zero;

        Vector3 targetPosition = m_currentPath[m_currentNodeIndex];
        Vector3 direction = (targetPosition - transform.position).normalized;

        Vector3 combinedDirection = (direction + separationDirection).normalized;

        this.transform.position += combinedDirection * Time.deltaTime * m_speed;
        m_spriteRenderer.flipX = direction.x < 0;


        if (Vector2.Distance(targetPosition, transform.position) < 0.1f)
        {
            if (m_currentNodeIndex == m_currentPath.Count - 1)
            {
                m_currentPath = null;
                OnDestinationReached?.Invoke();
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
            PathNode newEndNode = pathFinding.FindClosestPathNode(destination.Value, m_currentPath[^1]);
            if (newEndNode == null) return;

            Vector3 endDestination = new Vector3(newEndNode.centerX, newEndNode.centerY);
            if (Vector3.Distance(endDestination, m_currentPath[^1]) < 0.1f) return;
        }

        m_currentPath = pathFinding.FindPath(this.transform.position, destination.Value);
        m_currentNodeIndex = 0;
    }

    private Vector3 CalculateSeparation()
    {
        Vector3 separationVector = Vector3.zero;
        float separationRadiusSqr = m_separationRadius * m_separationRadius;

        List<Unit> units = GameManager.Instance.GetUnits(m_unit.Class);

        foreach (Unit unit in units)
        {
            if (unit == m_unit) continue;

            Vector3 oppositeDirection = this.transform.position - unit.transform.position;
            float sqrDistance = oppositeDirection.sqrMagnitude;

            if (sqrDistance < separationRadiusSqr && sqrDistance > 0)
            {
                separationVector += oppositeDirection.normalized / sqrDistance;
            }
        }

        return separationVector * m_separationForce;
    }

    private bool IsPathValid()
    {
        return m_currentPath != null && m_currentNodeIndex < m_currentPath.Count;
    }
}