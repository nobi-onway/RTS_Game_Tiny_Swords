using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    [SerializeField] private ActionSO[] m_actionSOArray;
    [SerializeField] private float m_objectDetectionRadius;

    protected Animator animator;
    protected SpriteRenderer spriteRenderer;

    public ActionSO[] ActionSOArray => m_actionSOArray;

    protected virtual void Awake()
    {
        SetUpComponent<Animator>(ref animator);
        SetUpComponent<SpriteRenderer>(ref spriteRenderer);
    }

    protected void SetUpComponent<T>(ref T component) where T : Component
    {
        if (!TryGetComponent(out T componentTryToGet)) return;

        component = componentTryToGet;
    }

    private Collider2D[] RunProximityObjectDetection()
    {
        return Physics2D.OverlapCircleAll(transform.position, m_objectDetectionRadius);
    }

    protected bool IsCloseObject<T>(out T obj) where T : MonoBehaviour
    {
        Collider2D[] hits = RunProximityObjectDetection();


        foreach (Collider2D hit in hits)
        {
            if (!hit.TryGetComponent<T>(out T component)) continue;

            obj = component;
            return true;
        }

        obj = default(T);
        return false;
    }

    public void Select()
    {
        Highlight();
    }

    public void Deselect()
    {
        Unhighlight();
    }

    public virtual bool TryInteractWithOtherUnit(Unit unit) => false;

    private void Highlight()
    {
        spriteRenderer.material = ResourceManager.Instance.HighlightMaterial;
    }

    private void Unhighlight()
    {
        spriteRenderer.material = ResourceManager.Instance.OriginalMaterial;
    }
}