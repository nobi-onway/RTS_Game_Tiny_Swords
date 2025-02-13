using UnityEngine;

public class UIManager : SingletonManager<UIManager>
{
    [Header("UI")]
    [SerializeField] private PointToClick m_pointToClickPrefab;
    [SerializeField] private ActionBar m_actionBar;
    [SerializeField] private ConfirmationBar m_confirmationBar;
    private PointToClick m_pointToClick;

    private void Start()
    {
        GameManager.Instance.OnMoveActiveUnit += DisplayClickEffect;
        GameManager.Instance.OnDeselectUnit += HideActionBar;
        GameManager.Instance.OnSelectUnit += ShowActionBar;

        GameManager.Instance.OnSelectBuildingUnit += HandleSelectBuildingUI;
        GameManager.Instance.OnCancelSelectBuildingUnit += HideConfirmationBar;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnMoveActiveUnit -= DisplayClickEffect;
        GameManager.Instance.OnDeselectUnit -= HideActionBar;
        GameManager.Instance.OnSelectUnit -= ShowActionBar;

        GameManager.Instance.OnSelectBuildingUnit -= HandleSelectBuildingUI;
        GameManager.Instance.OnCancelSelectBuildingUnit -= HideConfirmationBar;
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

    private void HandleSelectBuildingUI(BuildingSO buildingSO)
    {
        m_confirmationBar.Show(buildingSO.GoldCost, buildingSO.WoodCost);
    }

    private void HideConfirmationBar()
    {
        m_confirmationBar.Hide();
    }

}