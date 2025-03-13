using UnityEngine;

public class BuildingUnit : Unit
{
    private BuildingSO m_buildingSO;
    private PlacementProcess m_placementProcess;
    private BuildingProcess m_buildingProcess;
    public bool IsUnderConstruct => !m_buildingProcess.IsConstructCompleted;
    private CapsuleCollider2D m_collider2D;

    protected override void Start()
    {

    }

    protected override void Awake()
    {
        base.Awake();

        GeneralUtils.SetUpComponent<CapsuleCollider2D>(transform, ref m_collider2D);
    }

    private void Update()
    {
        UpdatePlacementProcess();
    }

    private void UpdatePlacementProcess()
    {
        if (m_buildingProcess != null) return;

        m_placementProcess.Update(m_buildingSO);

        InputManager.Instance.IsLockPan = true;
    }

    public void UnassignWorkerUnit(WorkerUnit workerUnit)
    {
        m_buildingProcess.RemoveWorker(workerUnit);
    }

    public bool TryStartBuildProgress(WorkerUnit workerUnit)
    {
        InputManager.Instance.IsLockPan = false;

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

            GameManager.Instance.RegisterUnit(this);
            return true;
        }

        return false;
    }

    private void UpdateCollider(Vector2 size)
    {
        m_collider2D.size = size;
        m_collider2D.offset = new Vector2(0, size.y / 2);
    }

    public void AssignWorkerUnit(WorkerUnit workerUnit)
    {
        m_buildingProcess.TryAddWorker(workerUnit, this);
    }

    public void UpdateBuildingProgress(float progress)
    {
        m_buildingProcess.Update(progress, UpdateBuildingArea);
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

        UpdateCollider(new Vector2(m_collider2D.size.x, m_collider2D.size.y * 2));
    }

    public override bool TryInteractWithOtherUnit(Unit unit)
    {
        if (!base.TryInteractWithOtherUnit(unit)) return false;

        return false;
    }
}