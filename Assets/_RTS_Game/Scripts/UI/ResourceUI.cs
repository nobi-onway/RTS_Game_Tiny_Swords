using TMPro;
using UnityEngine;

public class ResourceUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_goldTMP;
    [SerializeField] private TextMeshProUGUI m_woodTMP;

    private void OnEnable()
    {
        PlayerResourceManager.Instance.OnResourceChange += HandleOnResourceChange;
    }

    private void OnDisable()
    {
        PlayerResourceManager.Instance.OnResourceChange -= HandleOnResourceChange;
    }

    private void HandleOnResourceChange(int curGold, int curWood)
    {
        m_goldTMP.text = curGold.ToString();
        m_woodTMP.text = curWood.ToString();
    }
}