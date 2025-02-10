using UnityEngine;

public class AIPawn : MonoBehaviour
{
    [SerializeField]
    private float m_speed;

    private Vector3? m_destination;

    private void Update()
    {
        if (m_destination.HasValue)
        {
            Vector3 direction = m_destination.Value - transform.position;
            this.transform.position += direction.normalized * Time.deltaTime * m_speed;

            m_destination = Vector2.Distance(m_destination.Value, transform.position) > 0.1f ? m_destination : null;
        }
    }

    public void SetDestination(Vector3 destination)
    {
        m_destination = destination;
    }
}