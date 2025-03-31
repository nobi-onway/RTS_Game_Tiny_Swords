using UnityEngine;

[CreateAssetMenu(menuName = "SO/BuildingSO", fileName = "BuildingSO")]
public class BuildingSO : ScriptableObject
{
    [SerializeField] private Sprite m_placementSprite, m_foundationSprite, m_completionSprite;
    [SerializeField] private int m_goldCost, m_woodCost;
    [SerializeField] private Vector2Int m_buildingSize;
    [SerializeField] private Vector3Int m_buildingOffset;
    [SerializeField] private float m_buildingTime;
    [SerializeField] private BuildingUnit m_buildingUnitPrefab;
    [SerializeField] private ParticleSystem m_constructEffectPrefab;
    [SerializeField] private string m_description;

    public Sprite PlacementSprite => m_placementSprite;
    public Sprite FoundationSprite => m_foundationSprite;
    public Sprite CompletionSprite => m_completionSprite;
    public int GoldCost => m_goldCost;
    public int WoodCost => m_woodCost;
    public Vector2Int BuildingSize => m_buildingSize;
    public Vector3Int BuildingOffset => m_buildingOffset;
    public float BuildingTime => m_buildingTime;
    public BuildingUnit BuildingUnitPrefab => m_buildingUnitPrefab;
    public ParticleSystem BuildingEffectPrefab => m_constructEffectPrefab;
    public string Description => m_description;
}