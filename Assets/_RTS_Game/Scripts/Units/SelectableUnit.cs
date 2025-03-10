using UnityEngine;

public class SelectableUnit : MonoBehaviour
{
    [SerializeField] protected ActionSO[] m_actionSOArray;
    private SpriteRenderer m_spriteRenderer;

    private void Awake()
    {
        GeneralUtils.SetUpComponent<SpriteRenderer>(this.transform, ref m_spriteRenderer);
    }
    public void Select()
    {
        Highlight();

        UIManager.Instance.ShowActionBar(m_actionSOArray);
    }

    public void Deselect()
    {
        Unhighlight();

        UIManager.Instance.HideActionBar();
    }

    private void Highlight()
    {
        m_spriteRenderer.material = ResourceManager.Instance.HighlightMaterial;
    }

    private void Unhighlight()
    {
        m_spriteRenderer.material = ResourceManager.Instance.OriginalMaterial;
    }
}