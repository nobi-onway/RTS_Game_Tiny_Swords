using UnityEngine;

public class StructureUnit : Unit
{
    [SerializeField] private bool m_canStoreWood = false;
    [SerializeField] private bool m_canStoreGold = false;

    public bool CanStoreWood => m_canStoreWood;
    public bool CanStoreGold => m_canStoreGold;

    public override bool Enable => CanStoreWood;

    protected override void Start()
    {
        GameManager.Instance.RegisterUnit(this);
    }

    protected override void UnregisterUnit()
    {
        GameManager.Instance.UnregisterUnit(this);
    }
}