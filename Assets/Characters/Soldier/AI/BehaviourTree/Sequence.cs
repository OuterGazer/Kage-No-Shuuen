using System.Collections;
using System.Collections.Generic;

namespace BehaviourTree
{
    public class Sequence : Node // 'And' Node. It will run children in sequence upon its own success' states.
                                 // FAILURE at any time from any child (currently running or previous) means automatic failure if this node
                                 // Previous SUCCESS nodes will still be evaluated before any RUNNING node later in the list
    {
        public Sequence() : base() { }
        public Sequence(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            foreach(Node item in children)
            {
                // Known bugs: child nodes who have succeeded will still be evaluated when next nodes in the sequence are running
                switch (item.Evaluate())
                {
                    case NodeState.FAILURE:
                        state = NodeState.FAILURE;
                        return state;
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.RUNNING:
                        state = NodeState.RUNNING;
                        return state;
                    default:
                        state = NodeState.SUCCESS;
                        return state;
                }
            }
            state = NodeState.SUCCESS;
            return state;
        }
    }
}