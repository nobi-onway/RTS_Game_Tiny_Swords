using System;
using UnityEngine;
public abstract class ActionSO : ScriptableObject
{
    public Sprite Icon;
    public string ActionName;
    public string Guid => ActionName.GetHashCode().ToString();
    public ActionCard ActionCardPrefab;
    public virtual void PrepareExecute() { }
    public virtual void Execute()
    {
        Unit unit = GameManager.Instance.ActiveUnit;

        if (!unit.TryGetComponent(out SelectableUnit selectableUnit)) return;

        selectableUnit.SetCurrentActionIdx(this);
    }
}