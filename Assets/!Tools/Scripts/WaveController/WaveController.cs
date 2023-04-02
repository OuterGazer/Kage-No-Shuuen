using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class WaveController : MonoBehaviour
{
    private ObjectPool[] objectPools;

    [SerializeField] Wave[] waves;
    [SerializeField] AudioClip playerIsBusted;

    public List<Transform> currentWaveSoldiers = new();

    public UnityEvent onWavesEnded;

    private AudioSource audioSource;

    private void Awake()
    {
        objectPools = GetComponentsInChildren<ObjectPool>();
        audioSource = GetComponent<AudioSource>();
    }

    public void StartWaves()
    {
        audioSource.PlayOneShot(playerIsBusted);
        SoundManager.Instance.ChangeToEnemyWavesMusic();
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
                GameObject soldier = objectPool.GetObject(waves[i].patrolParents[j]);

                currentWaveSoldiers.Add(soldier.transform);

                yield return new WaitForSeconds(waves[i].timeBetweenSpawnings);
            }

            yield return new WaitUntil(() => currentWaveSoldiers.Count < 1);
        }

        onWavesEnded.Invoke();
        SoundManager.Instance.PlayVictoryMusic();

    }
}
