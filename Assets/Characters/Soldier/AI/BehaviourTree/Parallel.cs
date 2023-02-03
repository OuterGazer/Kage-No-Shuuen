using System.Collections;
using System.Collections.Generic;

namespace BehaviourTree
{
    public class Parallel : Node // This node will run several children in parallel and sequence upon its own running/success states.
                                 // Failure at any time from any child means automatic failure if this node
    {
        public Parallel() : base() { }
        public Parallel(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            bool isAnyChildRunning = false;

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
                        isAnyChildRunning = true;
                        continue;
                    default:
                        state = NodeState.SUCCESS;
                        return state;
                }
            }

            state = isAnyChildRunning ? NodeState.RUNNING : NodeState.SUCCESS;
            return state;
        }
    }
}