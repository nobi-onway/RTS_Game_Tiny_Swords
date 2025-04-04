using UnityEngine;

public class StorageUnit : Unit
{
    [SerializeField] private bool m_canStoreWood = false;
    [SerializeField] private bool m_canStoreGold = false;

    public bool CanStoreWood => m_canStoreWood;
    public bool CanStoreGold => m_canStoreGold;

    public override bool Enable => CanStoreWood || CanStoreGold;

    protected override void OnEnable()
    {
        base.OnEnable();

        PlayerResourceManager.Instance.OnAddResource += HandleOnAddResource;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        PlayerResourceManager.Instance.OnAddResource -= HandleOnAddResource;
    }

    protected override void Start()
    {
        GameManager.Instance.RegisterUnit(this);
    }

    protected override void UnregisterUnit()
    {
        GameManager.Instance.UnregisterUnit(this);
    }

    private void HandleOnAddResource(int gold, int wood)
    {
        if (CanStoreWood && wood > 0) UIManager.Instance.ShowTextPopup(GeneralUtils.GetTopPosition(this.transform), wood.ToString(), Color.black);
        if (CanStoreGold && gold > 0) UIManager.Instance.ShowTextPopup(GeneralUtils.GetTopPosition(this.transform), gold.ToString(), Color.yellow);
    }

    public override bool TryInteractWithOtherUnit(Unit unit)
    {
        if (!base.TryInteractWithOtherUnit(unit)) return false;

        return false;
    }
}