using UnityEngine;

[CreateAssetMenu(menuName = "SO/ActionSO", fileName = "ActionSO")]
public class BuildActionSO : ActionSO
{
    [SerializeField] private BuildingSO m_buildingSO;

    public BuildingSO BuildingSO => m_buildingSO;

    public override void Execute()
    {
        GameManager.Instance.PlaceActiveBuildingUnit();
        UIManager.Instance.HideBuildingConfirmationBar();
    }

    public override void PrepareExecute()
    {
        BuildingUnit buildingUnit = Instantiate(m_buildingSO.BuildingUnitPrefab);
        buildingUnit.SetUpBySO(
            m_buildingSO,
            TilemapManager.Instance
        );

        GameManager.Instance.SelectNewBuildingUnit(buildingUnit);
        UIManager.Instance.ShowBuildingConfirmationBar(m_buildingSO);
    }
}