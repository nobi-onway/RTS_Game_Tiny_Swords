using UnityEngine;

[CreateAssetMenu(menuName = "SO/ActionSO/StanceAction", fileName = "ActionSO")]
public class StanceActionSO : ActionSO
{
    [SerializeField] private ECombatStance m_combatStance;
    public ECombatStance CombatStance => m_combatStance;

    public override void Execute()
    {
        base.Execute();

        Unit unit = GameManager.Instance.ActiveUnit;

        if (unit is not CombatUnit combatUnit) return;

        combatUnit.StanceSystem.SetValue(m_combatStance);
    }
}