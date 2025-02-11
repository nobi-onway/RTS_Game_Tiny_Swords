using Unity.VisualScripting;
using UnityEngine;

public class BuildingUnit : MonoBehaviour
{
    private BuildingSO m_buildingSO;
    private SpriteRenderer m_spriteRenderer;

    private void Update()
    {
        if (InputManager.Instance.TryGetHoldWorldPosition(out Vector2 worldPosition))
        {
            this.transform.position = GridUtils.SnapToGrid(worldPosition);
        }
    }

    public void SetUpBySO(BuildingSO buildingSO)
    {
        m_buildingSO = buildingSO;
        m_spriteRenderer = this.AddComponent<SpriteRenderer>();

        m_spriteRenderer.sprite = m_buildingSO.PlacementSprite;
        m_spriteRenderer.color = new Color(1, 1, 1, 0.5f);
        m_spriteRenderer.sortingLayerName = "Unit";
    }
}