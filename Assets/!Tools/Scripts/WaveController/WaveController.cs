using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class WaveController : MonoBehaviour
{
    private ObjectPool[] objectPools;

    [SerializeField] Wave[] waves;

    [SerializeField] Transform[] patrolParents;

    public List<Transform> currentWaveSoldiers = new();

    private void Awake()
    {
        objectPools = GetComponentsInChildren<ObjectPool>();
    }

    public void StartWaves()
    {
        StartCoroutine(SpawnWaves());
    }

    private IEnumerator SpawnWaves()
    {
        for (int i = 0; i < waves.Length; i++)
        {
            for(int j = 0; j < waves[i].soldiers.Length; j++)
            {
                SoldierType soldierType = waves[i].soldiers[j];
                ObjectPool objectPool = objectPools.First(x => x.Prefab.GetComponent<SoldierBehaviour>().Type == soldierType);
                GameObject soldier = objectPool.GetObject();

                soldier.GetComponent<TaskPatrol>().SetPatrolParent(patrolParents[j]);

                soldier.GetComponent<NavMeshAgent>().Warp(patrolParents[j].transform.position);

                currentWaveSoldiers.Add(soldier.transform);

                yield return new WaitForSeconds(waves[i].timeBetweenSpawnings);
            }

            yield return new WaitUntil(() => currentWaveSoldiers.Count < 1);
        }
    }

    // TODO: hacer que ReturnToPool() se ejecute al terminar la animación de muerte
    //       Debo volver a reactivar los collisionadores (mirar StealthState y revisar todo lo que desactivo para volverlo a activar)
    //       De alguna manera los soldados reaparecen con vida negativa
    //       Debo hacer bien la asignación de la patrulla pq en la reaparición se quedan quietos
}
