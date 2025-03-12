using System.Linq;
using UnityEngine;

public class SelectableUnit : MonoBehaviour
{
    [SerializeField] protected ActionSO[] m_actionSOArray;
    [SerializeField] protected ActionSO m_defaultActionSO;
    [SerializeField] private int m_currentActionIdx;
    private SpriteRenderer m_spriteRenderer;

    private void Awake()
    {
        GeneralUtils.SetUpComponent<SpriteRenderer>(this.transform, ref m_spriteRenderer);

        m_currentActionIdx = GetActionIdx(m_defaultActionSO);
    }

    public void Select()
    {
        Highlight();

        UIManager.Instance.ShowActionBar(m_actionSOArray, m_currentActionIdx);
    }

    public void Deselect()
    {
        Unhighlight();

        UIManager.Instance.HideActionBar();
    }

    public void SetCurrentActionIdx(ActionSO actionSO)
    {
        m_currentActionIdx = GetActionIdx(actionSO);
    }

    public ActionSO GetCurrentActionSO() => m_actionSOArray[m_currentActionIdx];

    private int GetActionIdx(ActionSO requiredActionSO) => m_actionSOArray.ToList().FindIndex(actionSO => actionSO.Guid == requiredActionSO.Guid);

    private void Highlight()
    {
        m_spriteRenderer.material = ResourceManager.Instance.HighlightMaterial;
    }

    private void Unhighlight()
    {
        m_spriteRenderer.material = ResourceManager.Instance.OriginalMaterial;
    }
}