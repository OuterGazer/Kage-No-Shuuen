using System.Collections;
using System.Collections.Generic;

namespace BehaviourTree
{
    public class Selector : Node // 'Or' node. The failure of one state doesn't necessarily mean the failure of this node, as other child nodes can still run.
                                 // RUNNING prevents further nodes in the list to be run, but previous SUCCESS and FAILURE will still run.
    {
        public Selector() : base() { }
        public Selector(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            bool hasAnyChildFailed = false;

            foreach (Node item in children)
            {
                switch(item.Evaluate())
                {
                    case NodeState.FAILURE:
                        hasAnyChildFailed = true;
                        continue;
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.RUNNING:
                        state = NodeState.RUNNING;
                        return state;
                }
            }
            state = hasAnyChildFailed ? NodeState.FAILURE : NodeState.SUCCESS;
            return state;
        }
    }
}