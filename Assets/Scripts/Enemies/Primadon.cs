using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Primadon : EnemyEntity
{
    [Header("Boss Stats (set per prefab)")]
    [SerializeField] private float maxHP = 20000f;

    [Header("Movement (set per prefab)")]
    [SerializeField] private bool allowMovement = true;
    public List<Transform> targetPoints = new List<Transform>();
    [Range(0f, 1f)] public float moveChancePerCooldown = 0.5f;
    public float arriveThreshold = 0.25f;

    [Header("Auto-find Target Points (optional)")]
    [SerializeField] private bool autoFindTargetPoints = true;
    [SerializeField] private string targetPointTag = "PrimadonPoint";
    [SerializeField] private bool onlyAutoFindIfListEmpty = true;

    private EnemyPatternShooter ePS;
    private bool isMoving = false;

    [SerializeField] GameObject playerTarget;
    void Start()
    {
        // ✅ NEW: auto-fill targetPoints from tagged objects (doesn't change your core logic)
        TryAutoFindTargetPoints();

        enemyHP = maxHP;
        currentHP = enemyHP;

        movementSpeed = 10f;
        stopDistance = 0f;

        if (DifficultyManager.I != null)
            movementSpeed *= DifficultyManager.I.MoveSpeedMult;

        ePS = GetComponent<EnemyPatternShooter>();

        if (targetPoints != null && targetPoints.Count > 0 && allowMovement)
        {
            int idx = GetClosestTargetPointIndex();
            target = targetPoints[idx].position;
            transform.position = target;
        }

        StartCoroutine(HandleMovement());
    }

    void Update()
    {
        FindPlayer();
        FacePoint(playerTarget.transform.position);


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

    // -------------------- NEW HELPERS (setup only) --------------------



    void FacePoint(Vector2 point)
    {
        Vector2 dir = (point - (Vector2)transform.position);
        if (dir.sqrMagnitude < 0.0001f) return;

        // If your sprite faces UP by default:
        gameObject.transform.up = dir.normalized;

        // If your sprite faces RIGHT by default, use this instead:
        // transform.right = dir.normalized;
    }

    void FindPlayer()
    {
        if (playerTarget == null)
            playerTarget = GameObject.FindGameObjectWithTag("Player");
    }

    private void TryAutoFindTargetPoints()
    {
        if (!autoFindTargetPoints) return;

        if (onlyAutoFindIfListEmpty == false || targetPoints == null || targetPoints.Count == 0)
        {
            var gos = GameObject.FindGameObjectsWithTag(targetPointTag);
            if (gos == null || gos.Length == 0)
            {
                Debug.LogWarning($"[Primadon] No target points found with tag '{targetPointTag}'.");
                return;
            }

            if (targetPoints == null) targetPoints = new List<Transform>();
            targetPoints.Clear();

            for (int i = 0; i < gos.Length; i++)
                targetPoints.Add(gos[i].transform);

            // Stable order (optional): top-to-bottom, then left-to-right
            targetPoints.Sort((a, b) =>
            {
                if (a == null && b == null) return 0;
                if (a == null) return 1;
                if (b == null) return -1;

                int y = b.position.y.CompareTo(a.position.y); // higher first
                return (y != 0) ? y : a.position.x.CompareTo(b.position.x);
            });
        }
    }
}
