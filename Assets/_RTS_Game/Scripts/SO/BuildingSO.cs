using UnityEngine;

[CreateAssetMenu(menuName = "SO/BuildingSO", fileName = "BuildingSO")]
public class BuildingSO : ScriptableObject
{
    [SerializeField] private Sprite m_placementSprite, m_foundationSprite, m_completionSprite;
    [SerializeField] private int m_goldCost, m_woodCost;
    [SerializeField] private Vector2Int m_buildingSize;
    [SerializeField] private Vector3Int m_buildingOffset;

    public Sprite PlacementSprite => m_placementSprite;
    public Sprite FoundationSprite => m_foundationSprite;
    public Sprite CompletionSprite => m_completionSprite;
    public int GoldCost => m_goldCost;
    public int WoodCost => m_woodCost;
    public Vector2Int BuildingSize => m_buildingSize;
    public Vector3Int BuildingOffset => m_buildingOffset;
}