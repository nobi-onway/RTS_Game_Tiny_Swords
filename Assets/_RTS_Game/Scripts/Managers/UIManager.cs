using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : SingletonManager<UIManager>
{
    [Header("UI")]
    [SerializeField] private PointToClick m_pointToClickPrefab;
    private PointToClick m_pointToClick;

    [SerializeField] private PointToInteract m_pointToInteractPrefab;
    [SerializeField] private List<InteractTypeToSprite> m_interactTypeToSpriteList = new();
    [SerializeField] private TextPopup m_textPopupPrefab;
    [SerializeField] private ActionBar m_actionBar;
    [SerializeField] private ConfirmationBar m_confirmationBar;

    public void DisplayClickEffect(Vector2 position)
    {
        if (m_pointToClick == null) m_pointToClick = Instantiate(m_pointToClickPrefab);

        m_pointToClick.DisplayAt(position);
    }

    public void DisplayInteractEffect(Vector2 position, EInteractType interactType)
    {
        PointToInteract pointToInteractClone = Instantiate(m_pointToInteractPrefab);

        pointToInteractClone.DisplayAt(position, m_interactTypeToSpriteList[(int)interactType].sprite);
    }

    public void HideActionBar()
    {
        m_actionBar.Hide();
    }

    public void ShowActionBar(ActionSO[] actions, int curActionIdx)
    {
        m_actionBar.Show(actions, curActionIdx);
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

[Serializable]
public struct InteractTypeToSprite
{
    public EInteractType interactType;
    public Sprite sprite;
}