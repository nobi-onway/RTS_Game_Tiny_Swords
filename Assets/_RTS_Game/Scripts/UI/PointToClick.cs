using System;
using System.Collections;
using UnityEngine;

public class PointToClick : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float m_duration = 2f;
    [Range(0, 1)]
    [SerializeField] private float m_startFade;
    [SerializeField] private AnimationCurve m_animationCurve;
    protected SpriteRenderer m_spriteRenderer;
    private Coroutine m_effectCoroutine;
    private Vector3 m_initialScale;


    private void Awake()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_initialScale = this.transform.localScale;
    }

    public void DisplayAt(Vector2 position)
    {
        m_spriteRenderer.color = Color.white;
        this.transform.position = position;

        DoEffect();
    }

    private void DoEffect()
    {
        if (m_effectCoroutine != null)
        {
            StopCoroutine(m_effectCoroutine);
        }

        m_effectCoroutine = StartCoroutine(IE_Effect(OnEffectComplete));
    }

    private IEnumerator IE_Effect(Action onComplete = null)
    {
        float timer = 0.0f;

        while (timer < m_duration)
        {
            timer += Time.deltaTime;

            FadeOut(timer);
            ScaleInOut(timer % 1);

            yield return null;
        }

        onComplete?.Invoke();
    }

    protected virtual void OnEffectComplete()
    {
        m_effectCoroutine = null;
    }

    private void FadeOut(float timer)
    {
        if (timer < m_duration * m_startFade) return;

        float fadeProgress = (timer - m_duration * m_startFade) / (m_duration * (1 - m_startFade));
        m_spriteRenderer.color = new Color(1, 1, 1, 1 - fadeProgress);
    }

    private void ScaleInOut(float progress)
    {
        float scaleMultiplier = m_animationCurve.Evaluate(progress);

        this.transform.localScale = m_initialScale * scaleMultiplier;
    }
}