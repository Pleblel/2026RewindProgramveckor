using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject stationaryEnemy;
    [SerializeField] private GameObject chaseEnemy;
    [SerializeField] private GameObject sideEnemy;

    [Header("Spawn Points")]
    [SerializeField] private Transform stationarySpawnPoint;
    [SerializeField] private List<Transform> stationarySeatTargets = new List<Transform>();
    [SerializeField] private List<Transform> chaseEnemySpawns = new List<Transform>();
    [SerializeField] private List<Transform> sideEnemySpawns = new List<Transform>();

    [Header("Tracking (auto)")]
    [SerializeField] private List<GameObject> stationaryEnemies = new List<GameObject>();
    [SerializeField] private List<GameObject> chaseEnemies = new List<GameObject>();
    [SerializeField] private List<GameObject> sideEnemies = new List<GameObject>();

    public void CleanupNulls()
    {
        stationaryEnemies.RemoveAll(e => e == null);
        chaseEnemies.RemoveAll(e => e == null);
        sideEnemies.RemoveAll(e => e == null);
    }

    public int AliveStationaryCount { get { CleanupNulls(); return stationaryEnemies.Count; } }
    public int AliveChaseCount { get { CleanupNulls(); return chaseEnemies.Count; } }
    public int AliveSideCount { get { CleanupNulls(); return sideEnemies.Count; } }

    // ---------------- Stationary ----------------

    public void SpawnStationary(int amount)
    {
        if (stationaryEnemy == null || stationarySpawnPoint == null) return;
        if (stationarySeatTargets == null || stationarySeatTargets.Count == 0) return;

        amount = Mathf.Clamp(amount, 1, stationarySeatTargets.Count);

        // pick unique seats
        List<int> indices = new List<int>();
        for (int i = 0; i < stationarySeatTargets.Count; i++) indices.Add(i);

        for (int i = 0; i < amount; i++)
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

    // ---------------- Chase (burst) ----------------

    public IEnumerator SpawnChaseBurst(int amount, float gapSeconds)
    {
        if (chaseEnemy == null || chaseEnemySpawns == null || chaseEnemySpawns.Count == 0) yield break;

        amount = Mathf.Max(1, amount);
        gapSeconds = Mathf.Max(0f, gapSeconds);

        for (int i = 0; i < amount; i++)
        {
            Transform sp = chaseEnemySpawns[Random.Range(0, chaseEnemySpawns.Count)];
            GameObject enemyGO = Instantiate(chaseEnemy, sp.position, sp.rotation);
            chaseEnemies.Add(enemyGO);

            if (i < amount - 1 && gapSeconds > 0f)
                yield return new WaitForSeconds(gapSeconds);
        }
    }

    // ---------------- Side (burst) ----------------

    public IEnumerator SpawnSideBurst(int amount, float gapSeconds, float yOffsetMin, float yOffsetMax)
    {
        if (sideEnemy == null || sideEnemySpawns == null || sideEnemySpawns.Count == 0) yield break;

        amount = Mathf.Max(1, amount);
        gapSeconds = Mathf.Max(0f, gapSeconds);

        for (int i = 0; i < amount; i++)
        {
            Transform sp = sideEnemySpawns[Random.Range(0, sideEnemySpawns.Count)];
            float yOff = Random.Range(yOffsetMin, yOffsetMax);

            Vector3 pos = sp.position + new Vector3(0f, yOff, 0f);
            GameObject enemyGO = Instantiate(sideEnemy, pos, sp.rotation);
            sideEnemies.Add(enemyGO);

            if (i < amount - 1 && gapSeconds > 0f)
                yield return new WaitForSeconds(gapSeconds);
        }
    }
}
