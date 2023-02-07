using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    [System.Serializable]
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    public abstract class Node : MonoBehaviour
    {
        [SerializeField] protected Tree belongingTree;

        [Header("Node Specific Properties")]
        [SerializeField] protected NodeState state;

        public Node Parent; // useful to check for shared data among child nodes
        [SerializeReference] protected List<Node> children = new(); // useful for managing composite nodes more easily
        private Dictionary<string, object> dataContext= new(); // The actual collection that will hold shared data among children nodes
        public void SetData(string key, object value)
        {
            dataContext[key] = value; // if key exists, overwrites the value. dataContext.Add(key, value) will throw an exception if key already exists. Both are the same performancewise
        }

        public object GetData(string key)
        {
            object value = null;
            if(dataContext.TryGetValue(key, out value)) { return value; } // if value is in this same node, return it directly

            Node node = Parent;
            while(node != null) // if value isn't in this node, look recursively in the parent nodes until value is found or we hit the end of the tree (parent will be equals to null)
            {
                value = node.GetData(key);
                if(value != null) { return value; }
                node = node.Parent;
            }
            return null;
        }

        public bool ClearData(string key)
        {
            if (dataContext.ContainsKey(key)) 
            { 
                dataContext.Remove(key);
                return true; 
            }

            Node node = Parent;
            while (node != null)
            {
                bool isCleared = node.ClearData(key);
                if (isCleared) { return true; }
                node = node.Parent;
            }
            return false;
        }

        public abstract NodeState Evaluate();
    }
}
