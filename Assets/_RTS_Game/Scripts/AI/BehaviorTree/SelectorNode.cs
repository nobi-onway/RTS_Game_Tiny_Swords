using System.Collections.Generic;

public class SelectorNode : BehaviorNode
{
    private List<BehaviorNode> m_children;

    public SelectorNode(params BehaviorNode[] children)
    {
        m_children = new List<BehaviorNode>(children);
    }

    public override EStatusNode Execute()
    {
        foreach (BehaviorNode child in m_children)
        {
            EStatusNode status = child.Execute();
            if (status != EStatusNode.FAILURE) return status;
        }

        return EStatusNode.FAILURE;
    }
}