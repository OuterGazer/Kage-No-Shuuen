using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenuAttribute(fileName = "Shared Data", menuName = "Tree Shared Data", order = 0)]
public class BehaviourTreeSharedData : ScriptableObject
{
    [Header("Properties")]
    public float patrolSpeed = 2f;
    public float runningSpeed = 5f;
}
