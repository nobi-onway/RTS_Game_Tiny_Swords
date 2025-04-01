using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private HealthController m_healthController;
    [SerializeField] private Image m_healthBar;

    private void OnEnable()
    {
        m_healthController.OnHealthChange += HandleOnHealthChange;
        m_healthController.OnDead += HandleOnDead;
    }

    private void OnDisable()
    {
        m_healthController.OnHealthChange -= HandleOnHealthChange;
        m_healthController.OnDead -= HandleOnDead;
    }

    private void HandleOnHealthChange(int currentHealth)
    {
        float percentage = (float)currentHealth / m_healthController.MaxHealth;

        m_healthBar.fillAmount = percentage;
    }

    private void HandleOnDead()
    {
        this.gameObject.SetActive(false);
    }
}