using UnityEngine;

public class ResourceManager : MonoSingletonManager<ResourceManager>
{
    private Material m_originalMaterial;
    private Material m_highlightMaterial;

    public Material OriginalMaterial => m_originalMaterial;
    public Material HighlightMaterial => m_highlightMaterial;

    protected override void Awake()
    {
        base.Awake();

        m_originalMaterial = Resources.Load<Material>("Materials/Standard_Material");
        m_highlightMaterial = Resources.Load<Material>("Materials/Outline_Material");
    }
}