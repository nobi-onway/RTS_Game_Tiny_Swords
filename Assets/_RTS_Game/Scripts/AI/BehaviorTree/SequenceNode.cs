using System.Collections.Generic;
using UnityEngine;

public class SequenceNode : BehaviorNode
{
    private List<BehaviorNode> m_children;

    public SequenceNode(params BehaviorNode[] children)
    {
        m_children = new List<BehaviorNode>(children);
    }

    public override EStatusNode Execute()
    {
        foreach (BehaviorNode child in m_children)
        {
            EStatusNode status = child.Execute();
            if (status != EStatusNode.SUCCESS) return status;
        }

        return EStatusNode.SUCCESS;
    }
}