using System;
using UnityEngine;
public abstract class ActionSO : ScriptableObject
{
    public Sprite Icon;
    public string ActionName;
    public string Guid = System.Guid.NewGuid().ToString();
    public ActionCard ActionCardPrefab;
    public abstract void PrepareExecute();
    public abstract void Execute();
}