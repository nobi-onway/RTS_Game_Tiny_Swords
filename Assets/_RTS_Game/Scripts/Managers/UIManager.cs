using UnityEngine;

public class UIManager : SingletonManager<UIManager>
{
    [Header("UI")]
    [SerializeField] private PointToClick m_pointToClickPrefab;
    [SerializeField] private ActionBar m_actionBar;
    private PointToClick m_pointToClick;

    protected override void Awake()
    {
        base.Awake();

        GameManager.Instance.OnMoveActiveUnit += DisplayClickEffect;
        GameManager.Instance.OnDeselectUnit += HideActionBar;
        GameManager.Instance.OnSelectUnit += ShowActionBar;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnMoveActiveUnit -= DisplayClickEffect;
        GameManager.Instance.OnDeselectUnit -= HideActionBar;
        GameManager.Instance.OnSelectUnit -= ShowActionBar;
    }

    private void DisplayClickEffect(Vector2 position)
    {
        if (m_pointToClick == null) m_pointToClick = Instantiate(m_pointToClickPrefab);

        m_pointToClick.DisplayAt(position);
    }

    private void HideActionBar()
    {
        m_actionBar.Hide();
    }

    private void ShowActionBar(Unit unit)
    {
        m_actionBar.Show(unit.ActionSOArray);
    }

}