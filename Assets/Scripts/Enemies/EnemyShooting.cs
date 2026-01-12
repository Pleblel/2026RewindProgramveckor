using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatternShooter : MonoBehaviour
{
    public enum Pattern
    {
        Ring360,
        ConeBurst,
        AimedBurst,
        Spiral,
        MultiRingWaves
    }

    [Header("References")]
    [SerializeField] private EnemyBulletSuperClass bulletPrefab;
    [SerializeField] private EnemyBulletSuperClass ovalBulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform player; // optional; can auto-find by tag

    [Header("Pattern Pool (randomly pick from these)")]
    [SerializeField]
    private List<Pattern> allowedPatterns = new List<Pattern>
    {
        Pattern.Ring360,
        Pattern.ConeBurst,
        Pattern.AimedBurst,
        Pattern.Spiral,
        Pattern.MultiRingWaves
        
    };

    [Header("General")]
    [SerializeField] private float patternCooldown = 2.0f;
    [SerializeField] private bool randomizeEveryCycle = true;

    [Header("Ring 360")]
    [SerializeField] private int ringBulletCount = 36;
    [SerializeField] private float ringSpeed = 6f;
    [SerializeField] private float ringAngleOffset = 0f;

    [Header("Cone Burst (shotgun arc)")]
    [SerializeField] private int coneBulletCount = 10;
    [SerializeField] private float coneArcDegrees = 60f;
    [SerializeField] private float coneSpeed = 7f;
    [SerializeField] private float coneBaseAngle = 270f; // down by default (Unity 2D up=90, right=0)

    [Header("Aimed Burst")]
    [SerializeField] private int aimedCount = 8;
    [SerializeField] private float aimedArcDegrees = 25f;
    [SerializeField] private float aimedSpeed = 8f;

    [Header("Spiral")]
    [SerializeField] private float spiralDuration = 2.0f;
    [SerializeField] private float spiralFireRate = 0.06f;
    [SerializeField] private int spiralBulletsPerTick = 2;
    [SerializeField] private float spiralSpeed = 6.5f;
    [SerializeField] private float spiralDegreesPerTick = 12f;
    private float spiralAngle;

    [Header("Multi Ring Waves")]
    [SerializeField] private int waveRings = 3;
    [SerializeField] private float waveRingDelay = 0.25f;
    [SerializeField] private int waveRingCount = 24;
    [SerializeField] private float waveSpeed = 6f;

    private Coroutine loop;

    private void Awake()
    {
        if (firePoint == null) firePoint = transform;

        if (player == null)
        {
            var p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    private void OnEnable()
    {
        loop = StartCoroutine(ShootLoop());
    }

    private void OnDisable()
    {
        if (loop != null) StopCoroutine(loop);
    }

    private IEnumerator ShootLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(patternCooldown);

            if (allowedPatterns == null || allowedPatterns.Count == 0)
                continue;

            Pattern chosen = allowedPatterns[Random.Range(0, allowedPatterns.Count)];

            yield return FirePattern(chosen);
        }
    }

    private IEnumerator FirePattern(Pattern pattern)
    {
        switch (pattern)
        {
            case Pattern.Ring360:
                FireRing360(ringBulletCount, ringSpeed, ringAngleOffset);
                break;

            case Pattern.ConeBurst:
                FireCone(coneBulletCount, coneArcDegrees, coneSpeed, coneBaseAngle);
                break;

            case Pattern.AimedBurst:
                FireAimedBurst(aimedCount, aimedArcDegrees, aimedSpeed);
                break;

            case Pattern.Spiral:
                yield return FireSpiral();
                break;

            case Pattern.MultiRingWaves:
                yield return FireRingWaves();
                break;
        }
    }

    private void FireRing360(int count, float speed, float angleOffsetDeg)
    {
        if (count <= 0) return;

        float step = 360f / count;
        for (int i = 0; i < count; i++)
        {
            float a = angleOffsetDeg + step * i;
            Vector2 dir = AngleToDir(a);
            SpawnOvalBullet(dir, speed);
        }
    }

    private void FireCone(int count, float arcDeg, float speed, float baseAngleDeg)
    {
        if (count <= 0) return;

        if (count == 1)
        {
            SpawnBullet(AngleToDir(baseAngleDeg), speed);
            return;
        }

        float half = arcDeg * 0.5f;
        for (int i = 0; i < count; i++)
        {
            float t = i / (float)(count - 1);
            float a = Mathf.Lerp(baseAngleDeg - half, baseAngleDeg + half, t);
            SpawnBullet(AngleToDir(a), speed);
        }
    }

    private void FireAimedBurst(int count, float arcDeg, float speed)
    {
        if (player == null) return;

        Vector2 from = firePoint.position;
        Vector2 to = player.position;
        Vector2 aimDir = (to - from).normalized;

        float baseAngle = DirToAngle(aimDir);

        // small arc around aim direction
        FireCone(count, arcDeg, speed, baseAngle);
    }

    private IEnumerator FireSpiral()
    {
        float end = Time.time + spiralDuration;

        while (Time.time < end)
        {
            for (int i = 0; i < spiralBulletsPerTick; i++)
            {
                SpawnBullet(AngleToDir(spiralAngle), spiralSpeed);
                spiralAngle += spiralDegreesPerTick;
            }

            yield return new WaitForSeconds(spiralFireRate);
        }
    }

    private IEnumerator FireRingWaves()
    {
        float offset = Random.Range(0f, 360f);
        for (int i = 0; i < waveRings; i++)
        {
            FireRing360(waveRingCount, waveSpeed, offset + i * (360f / waveRingCount) * 0.5f);
            yield return new WaitForSeconds(waveRingDelay);
        }
    }

    private void SpawnBullet(Vector2 dir, float speed)
    {
        EnemyBulletSuperClass b = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        b.Fire(dir, speed);
    }

    private void SpawnOvalBullet(Vector2 dir, float speed)
    {
        EnemyBulletSuperClass b = Instantiate(ovalBulletPrefab, firePoint.position, Quaternion.identity);
        b.Fire(dir, speed);
    }

    // 0° = right, 90° = up, 180° = left, 270° = down
    private static Vector2 AngleToDir(float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }

    private static float DirToAngle(Vector2 dir)
    {
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }
}
