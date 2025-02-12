using UnityEngine;
using UnityEngine.UI;

public abstract class ActionCard : MonoBehaviour
{
    [SerializeField] private Image m_icon;
    protected ActionSO m_actionSO;

    public virtual void Init(ActionSO actionSO)
    {
        m_actionSO = actionSO;
        m_icon.sprite = actionSO.Icon;
    }
}