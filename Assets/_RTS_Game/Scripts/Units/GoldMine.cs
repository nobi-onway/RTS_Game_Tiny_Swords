using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldMine : MonoBehaviour
{
    [SerializeField] private Sprite m_activeSprite;
    [SerializeField] private Sprite m_defaultSprite;

    [SerializeField] private CapsuleCollider2D m_collider;
    [SerializeField] private float m_enterMineFreq = 2.0f;
    [SerializeField] private float m_miningDuration = 2.0f;

    private SpriteRenderer m_spriteRenderer;

    private int m_maxAllowMiners = 2;
    private Queue<WorkerUnit> m_activeMinerQueue = new();
    private float m_nextPossibleEnterTime;

    private void Awake()
    {
        GeneralUtils.SetUpComponent<SpriteRenderer>(transform, ref m_spriteRenderer);
    }

    public bool TryToEnterMine(WorkerUnit worker)
    {
        bool allowMiner = m_activeMinerQueue.Count < m_maxAllowMiners;
        bool allowEnterTime = Time.time >= m_nextPossibleEnterTime;

        if (!allowMiner || !allowEnterTime) return false;

        worker.EnterMine();
        m_activeMinerQueue.Enqueue(worker);
        m_nextPossibleEnterTime = Time.time + m_enterMineFreq;
        StartCoroutine(IE_ReleaseWorkerAfterDelay(worker, m_miningDuration));

        ShowActiveSpriteIf(m_activeMinerQueue.Count > 0);

        return true;
    }

    private IEnumerator IE_ReleaseWorkerAfterDelay(WorkerUnit workerUnit, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (m_activeMinerQueue.Contains(workerUnit))
        {
            m_activeMinerQueue.Dequeue();
            workerUnit.OnLeaveMine();
        }

        ShowActiveSpriteIf(m_activeMinerQueue.Count > 0);
    }

    private void ShowActiveSpriteIf(bool active)
    {
        m_spriteRenderer.sprite = active ? m_activeSprite : m_defaultSprite;
    }
}