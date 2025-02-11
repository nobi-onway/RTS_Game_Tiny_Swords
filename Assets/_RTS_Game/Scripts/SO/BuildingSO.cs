using UnityEngine;

[CreateAssetMenu(menuName = "SO/BuildingSO", fileName = "BuildingSO")]
public class BuildingSO : ScriptableObject
{
    [SerializeField] private Sprite m_placementSprite, m_foundationSprite, m_completionSprite;
    [SerializeField] private int m_goldCost, m_woodCost;

    public Sprite PlacementSprite => m_placementSprite;
    public Sprite FoundationSprite => m_foundationSprite;
    public Sprite CompletionSprite => m_completionSprite;
    public int GoldCost => m_goldCost;
    public int WoodCost => m_woodCost;
}