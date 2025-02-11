using UnityEngine;

[CreateAssetMenu(menuName = "SO/ActionSO", fileName = "ActionSO")]
public class BuildActionSO : ActionSO
{
    [SerializeField] private BuildingSO m_buildingSO;

    public BuildingSO BuildingSO => m_buildingSO;

    public override void Execute()
    {
        BuildingUnit buildingUnit = new GameObject(m_buildingSO.name).AddComponent<BuildingUnit>();
        buildingUnit.SetUpBySO(m_buildingSO);
    }
}