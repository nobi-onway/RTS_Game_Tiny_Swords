using UnityEngine;

[CreateAssetMenu(menuName = "SO/ActionSO/StanceAction", fileName = "ActionSO")]
public class StanceActionSO : ActionSO
{
    [SerializeField] private ECombatStance m_warriorStance;
    public ECombatStance WarriorStance => m_warriorStance;

    public override void Execute()
    {
        base.Execute();

        Unit unit = GameManager.Instance.ActiveUnit;

        if (unit is not WarriorUnit warriorUnit) return;

        warriorUnit.StanceSystem.SetValue(m_warriorStance);
    }
}