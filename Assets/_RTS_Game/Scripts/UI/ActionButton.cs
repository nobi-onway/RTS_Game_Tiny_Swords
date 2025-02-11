using UnityEngine;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    [SerializeField] private Image m_icon;
    [SerializeField] private Button m_button;

    private void OnDestroy()
    {
        m_button.onClick.RemoveAllListeners();
    }

    public void Init(ActionSO actionSO)
    {
        m_icon.sprite = actionSO.Icon;
        m_button.onClick.AddListener(() => actionSO.Execute());
    }
}