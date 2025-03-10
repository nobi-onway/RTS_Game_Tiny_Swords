using System;
using UnityEngine;

public interface IActionNode
{
    public EStatusNode Execute(Blackboard blackboard, Action onSuccess = null);
}