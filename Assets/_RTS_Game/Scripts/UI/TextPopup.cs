using TMPro;
using UnityEngine;

public class TextPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_text;

    public void SetText(string text, Color color)
    {
        m_text.text = text;
        m_text.color = color;
    }
}