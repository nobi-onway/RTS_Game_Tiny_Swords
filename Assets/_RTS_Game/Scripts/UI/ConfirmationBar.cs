using TMPro;
using UnityEngine;

public class ConfirmationBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_goldValueTMP;
    [SerializeField] private TextMeshProUGUI m_woodValueTMP;

    public void Show(int reqGold, int reqWood)
    {
        m_goldValueTMP.text = reqGold.ToString();
        m_woodValueTMP.text = reqWood.ToString();

        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}