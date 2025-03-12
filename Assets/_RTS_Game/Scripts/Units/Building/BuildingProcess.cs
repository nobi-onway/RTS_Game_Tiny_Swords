using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingProcess
{
    private SpriteRenderer m_spriteRenderer;
    private Sprite m_completionSprite;
    private float m_buildingTime;
    private float m_buildingProgress = 0;
    private List<WorkerUnit> m_workerUnits = new();
    private ParticleSystem m_constructEffect;
    public bool IsConstructCompleted => m_buildingProgress >= m_buildingTime;
    private bool m_IsUnderConstruction => m_workerUnits.Exists(worker => worker.CurrentState == EUnitState.BUILDING);

    public BuildingProcess(Transform parent, Sprite foundationSprite, Sprite completionSprite, float buildingTime, ParticleSystem constructEffectPrefab, ref SpriteRenderer spriteRenderer)
    {
        m_buildingTime = buildingTime;
        m_completionSprite = completionSprite;
        m_constructEffect = UnityEngine.Object.Instantiate(constructEffectPrefab, parent.transform);

        InitVisual(parent, foundationSprite, ref spriteRenderer);
    }

    public void Update(float progress, Action OnBuildingFinished)
    {
        if (IsConstructCompleted)
        {
            OnBuildingCompleted();
            OnBuildingFinished?.Invoke();
            return;
        }

        m_buildingProgress += progress;
        if (!m_constructEffect.isPlaying) m_constructEffect.Play();
    }

    private void InitVisual(Transform parent, Sprite sprite, ref SpriteRenderer spriteRenderer)
    {
        GameObject structureGo = new GameObject("Building_Process");
        structureGo.transform.SetParent(parent);
        structureGo.transform.localPosition = Vector3.zero;

        m_spriteRenderer = structureGo.AddComponent<SpriteRenderer>();
        m_spriteRenderer.sprite = sprite;
        m_spriteRenderer.sortingLayerName = "Unit";

        spriteRenderer = m_spriteRenderer;
    }

    private void OnBuildingCompleted()
    {
        m_spriteRenderer.sprite = m_completionSprite;

        for (int i = 0; i < m_workerUnits.Count; i++)
        {
            m_workerUnits[i].UnassignFromBuildProcess();
            m_workerUnits.RemoveAt(i);
        }

        m_constructEffect.Stop();
    }

    public void RemoveWorker(WorkerUnit workerUnit)
    {
        workerUnit.UnassignFromBuildProcess();
        m_workerUnits.Remove(workerUnit);

        if (!m_IsUnderConstruction) m_constructEffect.Stop();
    }

    public bool TryAddWorker(WorkerUnit workerUnit, BuildingUnit buildingUnit)
    {
        if (m_workerUnits.Exists(worker => worker == workerUnit)) return false;

        workerUnit.AssignToBuildProcess(buildingUnit);
        m_workerUnits.Add(workerUnit);

        return true;
    }
}