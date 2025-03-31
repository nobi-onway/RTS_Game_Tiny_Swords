using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float m_claimUp, m_claimDown, m_claimLeft, m_claimRight;

    private void OnEnable()
    {
        InputManager.Instance.OnPanPosition += UpdatePosition;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnPanPosition -= UpdatePosition;
    }

    private void UpdatePosition(Vector2 deltaPosition, float panSpeed)
    {
        Vector3 translatePos = -deltaPosition * Time.deltaTime * panSpeed;

        if (translatePos.x < m_claimLeft - this.transform.position.x) translatePos.x = m_claimLeft - this.transform.position.x;
        if (translatePos.x > m_claimRight - this.transform.position.x) translatePos.x = m_claimRight - this.transform.position.x;
        if (translatePos.y < m_claimDown - this.transform.position.y) translatePos.y = m_claimDown - this.transform.position.y;
        if (translatePos.y > m_claimUp - this.transform.position.y) translatePos.y = m_claimUp - this.transform.position.y;

        this.transform.Translate(translatePos);
    }
}