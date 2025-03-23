using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConfirmationBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_goldValueTMP;
    [SerializeField] private TextMeshProUGUI m_woodValueTMP;
    [SerializeField] private RectTransform m_confirmationButtons;
    [SerializeField] private Button m_confirmButton;
    [SerializeField] private Button m_cancelButton;
    private bool m_isShowing = false;
    private int m_reqGold, m_reqWood;
    private UnityAction OnConfirm;
    private UnityAction OnCancel;

    private void OnEnable()
    {
        PlayerResourceManager.Instance.OnResourceChange += HandlePlayerResourceChange;

        m_confirmButton.onClick.AddListener(() => OnConfirm());
        m_cancelButton.onClick.AddListener(() => OnCancel());
    }

    private void OnDisable()
    {
        UnsubscribeAllAction();

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

        m_confirmationButtons.gameObject.SetActive(OnConfirm != null && OnCancel != null);
    }

    public void SetUpHooks(UnityAction onConfirm, UnityAction onCancel)
    {
        this.OnConfirm = onConfirm;
        this.OnCancel = onCancel;
    }

    private void UnsubscribeAllAction()
    {
        this.OnCancel = null;
        this.OnConfirm = null;

        m_confirmButton.onClick.RemoveAllListeners();
        m_cancelButton.onClick.RemoveAllListeners();
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