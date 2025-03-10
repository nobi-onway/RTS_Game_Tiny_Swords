using System;

public class ActionNode : BehaviorNode
{
    private IActionNode actionNode;
    private Blackboard blackboard;
    private Action onExecute;

    public ActionNode(IActionNode actionNode, Blackboard blackboard)
    {
        this.actionNode = actionNode;
        this.blackboard = blackboard;
    }

    public ActionNode(IActionNode actionNode, Blackboard blackboard, Action onExecute)
    {
        this.actionNode = actionNode;
        this.blackboard = blackboard;
        this.onExecute = onExecute;
    }

    public override EStatusNode Execute()
    {
        return actionNode.Execute(blackboard, onExecute);
    }
}