using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
   [SerializeField] GameObject stationaryEnemy;
   [SerializeField] GameObject chaseEnemy;
   [SerializeField] List<GameObject> stationaryEnemies = new List<GameObject>();
   [SerializeField] List<Transform> stationarySeatTargets;
   [SerializeField] Transform stationarySpawnPoint;
    public bool stationaryEnemiesAllowed;

    [SerializeField] int minRespawnTime = 4;
    [SerializeField] int maxRespawnTime = 9;

    [SerializeField] int spawnAmount = 2;   // how many to spawn when timer ends
    float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CleanupNulls();

        if (stationaryEnemies.Count > 0)
            return;

        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            return;
        }

        // Timer finished -> spawn + reset timer for next time (after they die again)
        SpawnStationaryEnemies();
        timer = Random.Range(minRespawnTime, maxRespawnTime + 1); // int range inclusive
    }


    void SpawnStationaryEnemies()
    {
        List<int> indices = new List<int>();
        for(int i = 0; i < stationarySeatTargets.Count; i++)
        {
            indices.Add(i);
        }


        for (int i = 0; i < spawnAmount; i++)
        {
            int pick = Random.Range(0, indices.Count);
            int seatIndex = indices[pick];
            indices.RemoveAt(pick);

            Transform seat = stationarySeatTargets[seatIndex];

            GameObject enemyGO = Instantiate(stationaryEnemy, stationarySpawnPoint.position, stationarySpawnPoint.rotation);
            stationaryEnemies.Add(enemyGO);

            var enemy = enemyGO.GetComponent<StationaryShootingEnemy>();
            if (enemy != null)
                enemy.FindTargetPosition(seat);
        }
    }

    void CleanupNulls()
    {
        stationaryEnemies.RemoveAll(e => e == null);
    }

}
