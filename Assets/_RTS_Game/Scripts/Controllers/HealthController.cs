using System;
using System.Collections;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [Header("Health Information")]
    [SerializeField] private int m_maxHealth;
    [SerializeField] private int m_currentHealth;
    public int CurrentHealth
    {
        get => m_currentHealth;
        private set
        {
            m_currentHealth = value;
            OnHealthChange?.Invoke(value);
        }
    }

    public event Action<int> OnHealthChange;
    public event Action OnDead;

    [Space]

    [Header("Flash Damage Effect")]
    [SerializeField] private Color m_damageFlashColor = new Color(1f, 0.4f, 0.4f, 1f);
    [SerializeField] private float m_flashDamageEffectDuration = 0.2f;
    [SerializeField] private int m_flashCount = 2;

    private Animator m_animator;
    private SpriteRenderer m_spriteRenderer;

    private void Awake()
    {
        GeneralUtils.SetUpComponent<Animator>(this.transform, ref m_animator);
        GeneralUtils.SetUpComponent<SpriteRenderer>(this.transform, ref m_spriteRenderer);
    }

    private void Start()
    {
        m_currentHealth = m_maxHealth;
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;

        StartCoroutine(IE_FlashEffect(m_flashDamageEffectDuration, m_flashCount, m_damageFlashColor));
        UIManager.Instance.SpawnTextPopup(GeneralUtils.GetTopPosition(this.transform), damage.ToString(), Color.red);

        if (CurrentHealth <= 0) Die();
    }

    private IEnumerator IE_FlashEffect(float duration, int flashCount, Color color)
    {
        Color originalColor = m_spriteRenderer.color;

        for (int i = 0; i < flashCount; i++)
        {
            m_spriteRenderer.color = color;
            yield return new WaitForSeconds(duration / flashCount);

            m_spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(duration / flashCount);
        }
    }

    private void Die()
    {
        OnDead?.Invoke();

        m_animator.SetTrigger(AnimatorParameter.DEAD_TRIG);
    }
}