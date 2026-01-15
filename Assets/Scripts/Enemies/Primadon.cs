using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Primadon : EnemyEntity
{
    [Header("Boss Stats (set per prefab)")]
    [SerializeField] private float maxHP = 20000f;

    [Header("Movement (set per prefab)")]
    [SerializeField] private bool allowMovement = true;
    public List<Transform> targetPoints = new List<Transform>();
    [Range(0f, 1f)] public float moveChancePerCooldown = 0.5f;
    public float arriveThreshold = 0.25f;

    private EnemyPatternShooter ePS;
    private bool isMoving = false;

    void Start()
    {
        enemyHP = maxHP;
        currentHP = enemyHP;

        movementSpeed = 10f;
        stopDistance = 0f;

        if (DifficultyManager.I != null)
            movementSpeed *= DifficultyManager.I.MoveSpeedMult;

        ePS = GetComponent<EnemyPatternShooter>();

        if (targetPoints != null && targetPoints.Count > 0)
        {
            int idx = GetClosestTargetPointIndex();
            target = targetPoints[idx].position;
            transform.position = target;
        }

        StartCoroutine(HandleMovement());
    }

    void Update()
    {
        if (!allowMovement) return;
        if (!isMoving) return;

        Movement(movementSpeed);

        if (Vector2.Distance(transform.position, target) <= arriveThreshold)
        {
            transform.position = target;
            isMoving = false;
        }
    }

    protected override void Movement(float speed)
    {
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    IEnumerator HandleMovement()
    {
        while (true)
        {
            float cd = (ePS != null) ? Mathf.Max(0.01f, ePS.patternCooldown) : 2f;
            yield return new WaitForSeconds(cd);

            if (!allowMovement) continue;
            if (isMoving) continue;
            if (targetPoints == null || targetPoints.Count < 2) continue;

            if (ePS != null && ePS.IsShooting) continue;

            if (Vector2.Distance(transform.position, target) > arriveThreshold) continue;

            if (Random.value >= moveChancePerCooldown) continue;

            int currentIndex = GetClosestTargetPointIndex();
            int nextIndex = currentIndex;

            int safety = 0;
            while (nextIndex == currentIndex && safety++ < 50)
                nextIndex = Random.Range(0, targetPoints.Count);

            target = targetPoints[nextIndex].position;
            isMoving = true;
        }
    }

    int GetClosestTargetPointIndex()
    {
        int best = 0;
        float bestDist = float.PositiveInfinity;
        Vector2 pos = transform.position;

        for (int i = 0; i < targetPoints.Count; i++)
        {
            if (targetPoints[i] == null) continue;

            float d = Vector2.Distance(pos, targetPoints[i].position);
            if (d < bestDist)
            {
                bestDist = d;
                best = i;
            }
        }
        return best;
    }
}
