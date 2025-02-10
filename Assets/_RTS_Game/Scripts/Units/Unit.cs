using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    private Material m_highlightMaterial;
    private Material m_originalMaterial;

    protected Animator animator;
    protected AIPawn AIPawn;
    protected SpriteRenderer spriteRenderer;

    private void Awake()
    {
        SetUpComponent<Animator>(ref animator);
        SetUpComponent<AIPawn>(ref AIPawn);
        SetUpComponent<SpriteRenderer>(ref spriteRenderer);

        m_originalMaterial = spriteRenderer.material;
        m_highlightMaterial = Resources.Load<Material>("Materials/Outline_Material");
    }
    private void SetUpComponent<T>(ref T component) where T : Component
    {
        if (!TryGetComponent(out T componentTryToGet)) return;

        component = componentTryToGet;
    }

    public void MoveTo(Vector3 position)
    {
        Vector2 direction = (position - this.transform.position).normalized;
        spriteRenderer.flipX = direction.x < 0;

        AIPawn.SetDestination(position);
    }

    public void Select()
    {
        Highlight();
    }

    public void Deselect()
    {
        Unhighlight();
    }

    private void Highlight()
    {
        spriteRenderer.material = m_highlightMaterial;
    }

    private void Unhighlight()
    {
        spriteRenderer.material = m_originalMaterial;
    }
}