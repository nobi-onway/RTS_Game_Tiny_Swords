using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    [SerializeField] private float m_objectDetectionRadius;
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
    public virtual EUnitClass Class => EUnitClass.PLAYER;

    [SerializeField]
    protected Unit target;
    protected bool hasTarget => target != null;

    protected HealthController healthController;

    [SerializeField]
    protected EnumSystem<EUnitState> m_stateSystem = new();
    public EUnitState CurrentState => m_stateSystem.Value;
    private CapsuleCollider2D capsuleCollider2D;
    public CapsuleCollider2D Collider => capsuleCollider2D;

    protected UnitRadar m_unitRadar;

    public virtual bool Enable => true;


    protected virtual void Awake()
    {
        GeneralUtils.SetUpComponent<Animator>(transform, ref animator);
        GeneralUtils.SetUpComponent<SpriteRenderer>(transform, ref spriteRenderer);
        GeneralUtils.SetUpComponent<HealthController>(transform, ref healthController);
        GeneralUtils.SetUpComponent<CapsuleCollider2D>(transform, ref capsuleCollider2D);
        GeneralUtils.SetUpComponent<UnitRadar>(this.transform, ref m_unitRadar);
    }
    protected virtual void Start()
    {
        GameManager.Instance.RegisterUnit(this);
    }

    protected virtual void OnEnable()
    {
        healthController.OnDead += HandleOnDead;
        m_unitRadar.OnScanned += HandleOnScanned;
    }

    protected virtual void OnDisable()
    {
        healthController.OnDead -= HandleOnDead;
        m_unitRadar.OnScanned -= HandleOnScanned;
    }

    protected void SetTarget(Unit unit)
    {
        target = unit;

        HandleOnSetTarget(unit);
    }

    protected virtual void HandleOnSetTarget(Unit target) { }

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

    protected virtual void HandleOnDead()
    {
        m_stateSystem.SetValue(EUnitState.DEAD);

        m_unitRadar.Enabled = false;
        SetTarget(null);

        UnregisterUnit();
    }

    protected virtual void UnregisterUnit()
    {
        GameManager.Instance.UnregisterUnit(this);
    }

    protected virtual void HandleOnScanned(Unit unit)
    {
        SetTarget(unit);
    }

    //ANIMATION EVENT
    private void DestroyGameObject()
    {
        Destroy(gameObject);
    }

    protected bool CanPerformAction()
    {
        return CurrentState != EUnitState.DEAD;
    }

    public virtual bool TryInteractWithOtherUnit(Unit unit)
    {
        return CanPerformAction();
    }
    public virtual void DoActionAt(Vector2 position) { }
}