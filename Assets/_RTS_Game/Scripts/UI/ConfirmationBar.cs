using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConfirmationBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_goldValueTMP;
    [SerializeField] private TextMeshProUGUI m_woodValueTMP;
    [SerializeField] private TextMeshProUGUI m_descriptionTMP;
    [SerializeField] private RectTransform m_confirmationButtons;
    [SerializeField] private Button m_confirmButton;
    [SerializeField] private Button m_cancelButton;

    [SerializeField] private Color m_availableColor;
    [SerializeField] private Color m_nonAvailableColor;
    private bool m_isShowing = false;
    private int m_reqGold, m_reqWood;
    private UnityAction OnConfirm;
    private UnityAction OnCancel;

    private void OnEnable()
    {
        PlayerResourceManager.Instance.OnResourceChange += HandlePlayerResourceChange;

        m_confirmButton.onClick.AddListener(HandleOnConfirm);
        m_cancelButton.onClick.AddListener(HandleOnCancel);
    }

    private void OnDisable()
    {
        UnsubscribeAllAction();

        PlayerResourceManager.Instance.OnResourceChange -= HandlePlayerResourceChange;
    }

    public void Show(int reqGold, int reqWood, string description, UnityAction onConfirm, UnityAction onCancel)
    {
        m_reqGold = reqGold;
        m_reqWood = reqWood;

        m_goldValueTMP.text = reqGold.ToString();
        m_woodValueTMP.text = reqWood.ToString();
        m_descriptionTMP.text = description;

        m_isShowing = true;
        this.gameObject.SetActive(true);

        HandlePlayerResourceChange(PlayerResourceManager.Instance.Gold, PlayerResourceManager.Instance.Wood);

        m_confirmationButtons.gameObject.SetActive(onConfirm != null && onCancel != null);

        SetUpHooks(onConfirm, onCancel);
    }

    public void SetUpHooks(UnityAction onConfirm, UnityAction onCancel)
    {
        this.OnConfirm = onConfirm;
        this.OnCancel = onCancel;
    }

    private void HandleOnConfirm()
    {
        AudioManager.Instance.PlayButtonClick();
        OnConfirm?.Invoke();
    }

    private void HandleOnCancel()
    {
        AudioManager.Instance.PlayButtonClick();
        OnCancel?.Invoke();
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

        m_goldValueTMP.color = gold < m_reqGold ? m_nonAvailableColor : m_availableColor;
        m_woodValueTMP.color = wood < m_reqWood ? m_nonAvailableColor : m_availableColor;
    }
}