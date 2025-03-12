using TMPro;
using UnityEngine;

public class TextPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_text;
    [SerializeField] private float m_duration;

    [SerializeField] private AnimationCurve m_fontSizeCurve;
    [SerializeField] private AnimationCurve m_xOffSetCurve;
    [SerializeField] private AnimationCurve m_yOffSetCurve;
    [SerializeField] private AnimationCurve m_alphaCurve;

    private float m_randomXDirection;

    private float m_elapsedTime;

    public void SetText(string text, Color color)
    {
        m_text.text = text;
        m_text.color = color;
    }

    private void Start()
    {
        m_randomXDirection = Random.Range(-1, 2);
    }

    private void Update()
    {
        m_elapsedTime += Time.deltaTime;
        float normalizedTime = m_elapsedTime / m_duration;

        if (normalizedTime >= 1)
        {
            Destroy(this.gameObject);
            return;
        }

        m_text.fontSize += m_fontSizeCurve.Evaluate(normalizedTime);

        float alphaCurve = m_alphaCurve.Evaluate(normalizedTime);
        m_text.color = new Color(m_text.color.r, m_text.color.g, m_text.color.b, alphaCurve);

        float xOffSetCurve = m_xOffSetCurve.Evaluate(normalizedTime) * m_randomXDirection;
        float yOffSetCurve = m_yOffSetCurve.Evaluate(normalizedTime);
        this.transform.position += new Vector3(xOffSetCurve, yOffSetCurve, 0) * Time.deltaTime;
    }
}