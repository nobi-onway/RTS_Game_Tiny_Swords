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

    protected virtual void Awake()
    {
        GeneralUtils.SetUpComponent<Animator>(transform, ref animator);
        GeneralUtils.SetUpComponent<SpriteRenderer>(transform, ref spriteRenderer);
        GeneralUtils.SetUpComponent<HealthController>(transform, ref healthController);
    }
    protected virtual void Start()
    {
        GameManager.Instance.RegisterUnit(this);
    }

    protected virtual void OnEnable()
    {
        healthController.OnDead += HandleOnDead;
    }

    protected virtual void OnDisable()
    {
        healthController.OnDead -= HandleOnDead;
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

    protected virtual void HandleOnDead()
    {
        m_stateSystem.SetValue(EUnitState.DEAD);

        GameManager.Instance.UnregisterUnit(this);
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