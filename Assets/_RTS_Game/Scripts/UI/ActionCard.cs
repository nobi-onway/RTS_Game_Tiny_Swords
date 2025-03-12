using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class ActionCard : MonoBehaviour
{
    [SerializeField] private Image m_icon;
    [SerializeField] private Sprite m_focusedSprite;
    [SerializeField] private Sprite m_defaultSprite;
    [SerializeField] private Image m_image;
    public ActionSO m_actionSO;
    protected UnityAction<ActionSO> OnFocus;

    public virtual void Init(ActionSO actionSO)
    {
        m_actionSO = actionSO;
        m_icon.sprite = actionSO.Icon;
        m_image.sprite = m_defaultSprite;
    }

    public virtual void Init(ActionSO actionSO, UnityAction<ActionSO> OnFocus)
    {
        Init(actionSO);
        this.OnFocus = OnFocus;
    }

    public void Focus()
    {
        m_image.sprite = m_focusedSprite;
    }

    public void Unfocus()
    {
        m_image.sprite = m_defaultSprite;
    }
}