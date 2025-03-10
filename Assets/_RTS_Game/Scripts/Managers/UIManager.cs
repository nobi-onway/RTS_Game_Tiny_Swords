using UnityEngine;

public class UIManager : SingletonManager<UIManager>
{
    [Header("UI")]
    [SerializeField] private PointToClick m_pointToClickPrefab;
    [SerializeField] private TextPopup m_textPopupPrefab;
    [SerializeField] private ActionBar m_actionBar;
    [SerializeField] private ConfirmationBar m_confirmationBar;
    private PointToClick m_pointToClick;

    public void DisplayClickEffect(Vector2 position)
    {
        if (m_pointToClick == null) m_pointToClick = Instantiate(m_pointToClickPrefab);

        m_pointToClick.DisplayAt(position);
    }

    public void HideActionBar()
    {
        m_actionBar.Hide();
    }

    public void ShowActionBar(ActionSO[] actions)
    {
        m_actionBar.Show(actions);
    }

    public void ShowBuildingConfirmationBar(BuildingSO buildingSO)
    {
        m_confirmationBar.Show(buildingSO.GoldCost, buildingSO.WoodCost);
    }

    public void HideBuildingConfirmationBar()
    {
        m_confirmationBar.Hide();
    }

    public void SpawnTextPopup(Vector3 position, string text, Color color)
    {
        TextPopup textPopupClone = Instantiate(m_textPopupPrefab, position, Quaternion.identity);

        textPopupClone.SetText(text, color);
    }

}