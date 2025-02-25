using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class BuildingUnit : Unit
{
    private BuildingSO m_buildingSO;
    private PlacementProcess m_placementProcess;
    private BuildingProcess m_buildingProcess;

    public bool IsUnderConstruct => !m_buildingProcess.IsConstructCompleted;

    private void Update()
    {
        UpdatePlacementProcess();
    }

    private void UpdatePlacementProcess()
    {
        if (m_buildingProcess != null) return;

        m_placementProcess.Update(m_buildingSO);
    }

    public void UnassignWorkerUnit(WorkerUnit workerUnit)
    {
        m_buildingProcess.RemoveWorker(workerUnit);
    }

    public void AssignWorkerUnit(WorkerUnit workerUnit)
    {
        m_buildingProcess.TryAddWorker(workerUnit, this);
    }

    public void UpdateBuildingProgress(float progress)
    {
        m_buildingProcess.Update(progress, UpdateBuildingArea);
    }

    public bool TryStartBuildProgress(WorkerUnit workerUnit)
    {
        if (m_placementProcess.TryPlaceBuilding())
        {
            m_buildingProcess = new BuildingProcess(
                                                    this.transform,
                                                    m_buildingSO.FoundationSprite,
                                                    m_buildingSO.CompletionSprite,
                                                    m_buildingSO.BuildingTime,
                                                    m_buildingSO.BuildingEffectPrefab,
                                                    ref spriteRenderer
                                                );

            m_buildingProcess.TryAddWorker(workerUnit, this);
            return true;
        }

        return false;
    }

    public void SetUpBySO(BuildingSO buildingSO, TilemapManager tilemapManager)
    {
        m_buildingSO = buildingSO;
        m_placementProcess = new PlacementProcess(
                                                    this.transform,
                                                    buildingSO.PlacementSprite,
                                                    ref spriteRenderer,
                                                    tilemapManager
                                                );
    }

    private void UpdateBuildingArea()
    {
        int buildingWidthInTiles = 6;
        int buildingHeightInTiles = 6;

        float halfWidth = buildingWidthInTiles / 2;
        float halfHeight = buildingHeightInTiles / 2;

        Vector3Int startPosition = GridUtils.SnapToGrid(this.transform.position - new Vector3(halfWidth, halfHeight));

        TilemapManager.Instance.PathFinding.UpdateNodesInArea(startPosition, buildingWidthInTiles, buildingHeightInTiles);
    }
}