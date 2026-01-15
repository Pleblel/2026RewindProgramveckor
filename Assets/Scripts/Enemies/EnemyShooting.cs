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
        MultiRingWaves,

        CrossBurst,
        RandomSpray,
        PlayerRingTrap,
        SweepingFan,
        DoubleSpiral
    }

    public bool IsShooting { get; private set; }

    [Header("References")]
    [SerializeField] private EnemyBulletSuperClass bulletPrefab;
    [SerializeField] private EnemyBulletSuperClass ovalBulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform player;

    [Header("Pattern Pool")]
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
    public float patternCooldown = 2.0f;

    [Header("Ring 360")]
    [SerializeField] private int ringBulletCount = 36;
    [SerializeField] private float ringSpeed = 6f;
    [SerializeField] private float ringAngleOffset = 0f;

    [Header("Cone Burst")]
    [SerializeField] private int coneBulletCount = 10;
    [SerializeField] private float coneArcDegrees = 60f;
    [SerializeField] private float coneSpeed = 7f;
    [SerializeField] private float coneBaseAngle = 270f;

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

    // Boss patterns
    [Header("Cross Burst (boss)")]
    [SerializeField] private int crossDirections = 8;
    [SerializeField] private float crossSpeed = 7f;
    [SerializeField] private float crossAngleOffset = 0f;

    [Header("Random Spray (boss)")]
    [SerializeField] private int sprayCount = 40;
    [SerializeField] private float spraySpeed = 7f;
    [SerializeField] private float sprayMinAngle = 0f;
    [SerializeField] private float sprayMaxAngle = 360f;

    [Header("Player Ring Trap (boss)")]
    [SerializeField] private int trapCount = 32;
    [SerializeField] private float trapRadius = 2.5f;
    [SerializeField] private float trapHoldTime = 2.5f;
    [SerializeField] private float trapReleaseSpeed = 6f;
    [SerializeField] private float trapAngleOffset = 0f;
    [SerializeField] private bool trapUseOvalBullets = true;

    [Header("Trap: Boss shoots while ring holds")]
    [SerializeField] private float trapShotInterval = 0.35f;
    [SerializeField] private int trapAimedCount = 8;
    [SerializeField] private float trapAimedArc = 20f;
    [SerializeField] private float trapAimedSpeed = 9f;

    [Header("Trap VFX (rotation)")]
    [SerializeField] private bool trapRotate = true;
    [SerializeField] private float trapRotateDegPerSec = 90f;
    [SerializeField] private bool trapClockwise = true;

    [Header("Sweeping Fan (boss)")]
    [SerializeField] private float fanDuration = 2.0f;
    [SerializeField] private float fanFireRate = 0.08f;
    [SerializeField] private int fanConeCount = 12;
    [SerializeField] private float fanArc = 70f;
    [SerializeField] private float fanSpeed = 8f;
    [SerializeField] private float fanStartAngle = 210f;
    [SerializeField] private float fanEndAngle = 330f;

    [Header("Double Spiral (boss)")]
    [SerializeField] private float doubleSpiralDuration = 2.0f;
    [SerializeField] private float doubleSpiralFireRate = 0.06f;
    [SerializeField] private float doubleSpiralSpeed = 6.5f;
    [SerializeField] private float doubleSpiralDegreesPerTick = 14f;
    private float doubleSpiralAngle;

    private Coroutine loop;

    private float IntervalMult => (DifficultyManager.I != null) ? DifficultyManager.I.IntervalMult : 1f;

    private void Awake()
    {
        if (firePoint == null) firePoint = transform;

        if (player == null)
        {
            var p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    private void OnEnable() => loop = StartCoroutine(ShootLoop());

    private void OnDisable()
    {
        if (loop != null) StopCoroutine(loop);
        IsShooting = false;
    }

    private IEnumerator ShootLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(patternCooldown * IntervalMult);

            if (allowedPatterns == null || allowedPatterns.Count == 0)
                continue;

            Pattern chosen = allowedPatterns[Random.Range(0, allowedPatterns.Count)];

            IsShooting = true;
            yield return FirePattern(chosen);
            IsShooting = false;
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

            case Pattern.CrossBurst:
                FireCrossBurst(crossDirections, crossSpeed, crossAngleOffset);
                break;

            case Pattern.RandomSpray:
                FireRandomSpray(sprayCount, spraySpeed, sprayMinAngle, sprayMaxAngle);
                break;

            case Pattern.PlayerRingTrap:
                yield return FirePlayerRingTrapSequence();
                break;

            case Pattern.SweepingFan:
                yield return FireSweepingFan();
                break;

            case Pattern.DoubleSpiral:
                yield return FireDoubleSpiral();
                break;
        }
    }

    // ---------------- existing patterns ----------------

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

            yield return new WaitForSeconds(spiralFireRate * IntervalMult);
        }
    }

    private IEnumerator FireRingWaves()
    {
        float offset = Random.Range(0f, 360f);
        for (int i = 0; i < waveRings; i++)
        {
            FireRing360(waveRingCount, waveSpeed, offset + i * (360f / waveRingCount) * 0.5f);
            yield return new WaitForSeconds(waveRingDelay * IntervalMult);
        }
    }

    // ---------------- boss patterns ----------------

    private void FireCrossBurst(int directions, float speed, float angleOffset)
    {
        directions = Mathf.Max(1, directions);
        float step = 360f / directions;

        for (int i = 0; i < directions; i++)
        {
            float a = angleOffset + step * i;
            SpawnBullet(AngleToDir(a), speed);
        }
    }

    private void FireRandomSpray(int count, float speed, float minAngle, float maxAngle)
    {
        if (count <= 0) return;

        for (int i = 0; i < count; i++)
        {
            float a = Random.Range(minAngle, maxAngle);
            SpawnBullet(AngleToDir(a), speed);
        }
    }

    private class TrapBullet
    {
        public EnemyBulletSuperClass bullet;
        public float angleDeg;
        public Vector2 releaseDir;
    }

    private IEnumerator FirePlayerRingTrapSequence()
    {
        if (player == null) yield break;
        if (trapCount <= 0) yield break;

        Vector2 center = player.position; // locked center
        float step = 360f / trapCount;

        EnemyBulletSuperClass prefab = trapUseOvalBullets ? ovalBulletPrefab : bulletPrefab;

        List<TrapBullet> ring = new List<TrapBullet>(trapCount);
        for (int i = 0; i < trapCount; i++)
        {
            float a = trapAngleOffset + step * i;
            Vector2 dir = AngleToDir(a);
            Vector2 spawnPos = center + dir * trapRadius;

            var b = Instantiate(prefab, spawnPos, Quaternion.identity);
            b.Fire(Vector2.zero, 0f); // hold

            ring.Add(new TrapBullet { bullet = b, angleDeg = a, releaseDir = dir });
        }

        float end = Time.time + trapHoldTime;
        float nextShot = Time.time;

        while (Time.time < end)
        {
            if (Time.time >= nextShot)
            {
                FireAimedBurst(trapAimedCount, trapAimedArc, trapAimedSpeed);
                nextShot = Time.time + (trapShotInterval * IntervalMult);
            }

            if (trapRotate)
            {
                float sign = trapClockwise ? -1f : 1f;
                float delta = trapRotateDegPerSec * sign * Time.deltaTime;

                for (int i = 0; i < ring.Count; i++)
                {
                    if (ring[i].bullet == null) continue;

                    ring[i].angleDeg += delta;
                    Vector2 dir = AngleToDir(ring[i].angleDeg);
                    ring[i].releaseDir = dir;
                    ring[i].bullet.transform.position = center + dir * trapRadius;
                }
            }

            yield return null;
        }

        for (int i = 0; i < ring.Count; i++)
        {
            if (ring[i].bullet != null)
                ring[i].bullet.Fire(ring[i].releaseDir, trapReleaseSpeed);
        }
    }

    private IEnumerator FireSweepingFan()
    {
        bool startLeft = Random.value < 0.5f;
        float startA = startLeft ? fanStartAngle : fanEndAngle;
        float endA = startLeft ? fanEndAngle : fanStartAngle;

        float end = Time.time + fanDuration;

        while (Time.time < end)
        {
            float t = 1f - ((end - Time.time) / fanDuration);
            float baseAngle = Mathf.Lerp(startA, endA, t);

            FireCone(fanConeCount, fanArc, fanSpeed, baseAngle);
            yield return new WaitForSeconds(fanFireRate * IntervalMult);
        }
    }

    private IEnumerator FireDoubleSpiral()
    {
        float end = Time.time + doubleSpiralDuration;

        while (Time.time < end)
        {
            Vector2 d1 = AngleToDir(doubleSpiralAngle);
            Vector2 d2 = AngleToDir(doubleSpiralAngle + 180f);

            SpawnBullet(d1, doubleSpiralSpeed);
            SpawnBullet(d2, doubleSpiralSpeed);

            doubleSpiralAngle += doubleSpiralDegreesPerTick;
            yield return new WaitForSeconds(doubleSpiralFireRate * IntervalMult);
        }
    }

    // ---------------- spawn helpers ----------------

    private void SpawnBullet(Vector2 dir, float speed)
    {
        EnemyBulletSuperClass b = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        b.Fire(dir, speed); // scaling happens inside bullet
    }

    private void SpawnOvalBullet(Vector2 dir, float speed)
    {
        EnemyBulletSuperClass b = Instantiate(ovalBulletPrefab, firePoint.position, Quaternion.identity);
        b.Fire(dir, speed); // scaling happens inside bullet
    }

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
