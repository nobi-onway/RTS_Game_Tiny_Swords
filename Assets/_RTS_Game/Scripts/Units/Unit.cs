using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    [SerializeField] private float m_objectDetectionRadius;
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
    protected CapsuleCollider2D capsuleCollider2D;
    public virtual EUnitClass Class => EUnitClass.PLAYER;

    [SerializeField]
    protected Unit target;
    protected bool hasTarget => target != null;

    protected EnumSystem<EUnitState> m_stateSystem = new();
    public EUnitState CurrentState => m_stateSystem.Value;

    protected virtual void Awake()
    {
        GeneralUtils.SetUpComponent<Animator>(transform, ref animator);
        GeneralUtils.SetUpComponent<SpriteRenderer>(transform, ref spriteRenderer);
        GeneralUtils.SetUpComponent<CapsuleCollider2D>(transform, ref capsuleCollider2D);
    }
    private void Start()
    {
        GameManager.Instance.RegisterUnit(this);
    }
    private void OnDestroy()
    {
        GameManager.Instance.UnregisterUnit(this);
    }

    protected void SetTarget(Unit unit)
    {
        target = unit;
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

    public Vector3 GetTopPosition()
    {
        if (capsuleCollider2D == null) return this.transform.position;

        return this.transform.position + Vector3.up * capsuleCollider2D.size.y / 2;
    }

    public virtual bool TryInteractWithOtherUnit(Unit unit) => false;
    public virtual void DoActionAt(Vector2 position) { }
}