using UnityEngine;
using System;

// ---------------- Enemy Bullets ----------------

public class EnemyBulletSuperClass : MonoBehaviour
{
    [SerializeField] private float lifeTime = 8f;

    private Vector2 moveDir = Vector2.down;
    [SerializeField] protected float bulletSpeed = 8f;

    private Rigidbody2D rb;
    private bool hasFired = false;
    private bool scaledDefault = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        CancelInvoke();
        if (lifeTime > 0f)
            Invoke(nameof(Despawn), lifeTime);

        // If this bullet was spawned without calling Fire(), we still want it to move.
        if (!hasFired)
        {
            // scale default bulletSpeed once
            if (!scaledDefault && DifficultyManager.I != null)
            {
                bulletSpeed *= DifficultyManager.I.BulletSpeedMult;
                scaledDefault = true;
            }

            ApplyVelocity();
        }
    }

    public virtual void Fire(Vector2 dir, float speed)
    {
        hasFired = true;

        moveDir = dir.normalized;

        float mult = (DifficultyManager.I != null) ? DifficultyManager.I.BulletSpeedMult : 1f;
        bulletSpeed = speed * mult;

        ApplyVelocity();
    }

    private void ApplyVelocity()
    {
        if (rb != null)
            rb.velocity = moveDir * bulletSpeed;
    }

    protected virtual void Update()
    {
        // fallback if no rigidbody
        if (rb == null)
            transform.position += (Vector3)(moveDir * bulletSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            Destroy(gameObject);
    }

    private void Despawn()
    {
        Destroy(gameObject);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}

// ---------------- Player Bullets ----------------

public class PlayerBulletSuperClass : MonoBehaviour
{
    protected Vector2 moveDir;
    protected float speed;
    protected int baseDamage;

    protected int pierceRemaining;
    protected PlayerShoot ownerShoot;

    protected Score score;

    private void Start()
    {
        score = GameObject.Find("GameManager").GetComponent<Score>();

        if (score != null)
            Debug.Log("Found Score");
    }

    public void Init(Vector2 dir, float bulletSpeed, int damage, int pierce, PlayerShoot owner)
    {
        moveDir = dir.normalized;
        speed = bulletSpeed;
        baseDamage = damage;
        pierceRemaining = pierce;
        ownerShoot = owner;
        transform.up = moveDir;
    }

    protected virtual void Update()
    {
        Move();
    }

    protected virtual void Move()
    {
        transform.position += (Vector3)(moveDir * speed * Time.deltaTime);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;

        score.AddScore(1000);


        var enemy = collision.GetComponent<EnemyEntity>();
        if (enemy != null)
        {
            int dmg = ownerShoot != null ? ownerShoot.GetEffectiveDamage(enemy) : baseDamage;
            enemy.TakeDamage(dmg);
        }

        if (pierceRemaining > 0)
        {
            pierceRemaining--;
            return;
        }

        Destroy(gameObject);
    }

    protected virtual void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}


// ---------------- Enemy Base ----------------
public abstract class EnemyEntity : MonoBehaviour
{
    public static event Action<EnemyEntity> OnAnyEnemyKilled;

    [Header("Stats")]
    public float enemyHP;
    public float currentHP;
    protected float movementSpeed;
    protected float stopDistance;
    protected Vector2 target;

    [Header("Audio (2 sources on this enemy)")]
    [SerializeField] private AudioSource hitAudio;   // plays when taking damage (non-lethal or every hit)
    [SerializeField] private AudioSource deathAudio; // plays on death

    [Header("Score")]
    protected Score score;

    protected virtual void Awake()
    {
        // Score (safe)
        var gm = GameObject.Find("GameManager");
        if (gm != null) score = gm.GetComponent<Score>();

        // Auto-wire audio sources if not assigned
        if (hitAudio == null || deathAudio == null)
        {
            // If you have exactly 2 AudioSources on the enemy, this auto-assigns them.
            var sources = GetComponents<AudioSource>();
            if (sources != null && sources.Length > 0)
            {
                if (hitAudio == null) hitAudio = sources.Length >= 1 ? sources[0] : null;
                if (deathAudio == null) deathAudio = sources.Length >= 2 ? sources[1] : hitAudio;
            }
        }
    }

    protected virtual void Movement(float speed)
    {
        Vector2 pos = transform.position;

        if (Vector2.Distance(pos, target) <= stopDistance)
        {
            transform.position = target;
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    public void TakeDamage(float damage)
    {
        // Play hit sound (before death check)
        if (hitAudio != null)
            hitAudio.Play();

        currentHP -= damage;

        if (currentHP <= 0)
        {
            if (score != null) score.AddScore(30000);

            OnAnyEnemyKilled?.Invoke(this);
            Death();
        }
    }

    public virtual void Death()
    {
        // Play death sound even though we're about to destroy this GameObject
        if (deathAudio != null && deathAudio.clip != null)
        {
            // detach audio source so it can finish playing
            deathAudio.transform.SetParent(null, true);
            deathAudio.Play();

            // destroy the audio object after clip finishes
            Destroy(deathAudio.gameObject, deathAudio.clip.length + 0.05f);
        }

        Destroy(gameObject);
    }
}

