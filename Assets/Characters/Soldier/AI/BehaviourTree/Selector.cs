using System.Collections;
using System.Collections.Generic;

namespace BehaviourTree
{
    public class Selector : Node // 'Or' node. The failure of one state doesn't necessarily mean the failure of this node, as other child nodes can still run.
    {
        public Selector() : base() { }
        public Selector(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            foreach(Node item in children)
            {
                switch(item.Evaluate())
                {
                    case NodeState.FAILURE:
                        continue;
                    case NodeState.SUCCESS:
                        state = NodeState.SUCCESS;
                        return state;
                    case NodeState.RUNNING:
                        state = NodeState.RUNNING;
                        return state;
                    default:
                        continue;
                }
            }
            state = NodeState.FAILURE;
            return state;
        }
    }
}