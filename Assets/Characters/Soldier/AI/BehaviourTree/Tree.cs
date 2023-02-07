using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public abstract class Tree : MonoBehaviour
    {
        private Node root = null;

        protected void Start()
        {
            root = SetUpTree();
        }

        private void Update()
        {
           root?.Evaluate();
        }

        protected abstract Node SetUpTree();
    }
}
