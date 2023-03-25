using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "Waves/Soldier Wave", order = 0)]
public class Wave : ScriptableObject
{
    public SoldierType[] soldiers;

    public float timeBetweenSpawnings = 1f;

    public Transform[] patrolParents;
}
