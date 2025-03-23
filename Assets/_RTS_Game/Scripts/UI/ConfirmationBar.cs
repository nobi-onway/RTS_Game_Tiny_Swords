using TMPro;
using UnityEngine;

public class ConfirmationBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_goldValueTMP;
    [SerializeField] private TextMeshProUGUI m_woodValueTMP;

    private bool m_isShowing = false;
    private int m_reqGold, m_reqWood;

    private void OnEnable()
    {
        PlayerResourceManager.Instance.OnResourceChange += HandlePlayerResourceChange;
    }

    private void OnDisable()
    {
        PlayerResourceManager.Instance.OnResourceChange -= HandlePlayerResourceChange;
    }

    public void Show(int reqGold, int reqWood)
    {
        m_reqGold = reqGold;
        m_reqWood = reqWood;

        m_goldValueTMP.text = reqGold.ToString();
        m_woodValueTMP.text = reqWood.ToString();

        m_isShowing = true;
        this.gameObject.SetActive(true);

        HandlePlayerResourceChange(PlayerResourceManager.Instance.Gold, PlayerResourceManager.Instance.Wood);
    }

    public void Hide()
    {
        m_isShowing = false;
        this.gameObject.SetActive(false);
    }

    private void HandlePlayerResourceChange(int gold, int wood)
    {
        if (!m_isShowing) return;

        m_goldValueTMP.color = gold < m_reqGold ? Color.red : Color.green;
        m_woodValueTMP.color = wood < m_reqWood ? Color.red : Color.green;
    }
}