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
    protected float bulletSpeed;
    protected int damage;
    protected Vector2 moveDir = Vector2.up;

    public void Init(Vector2 dir, float speed, int d)
    {
        moveDir = dir.normalized;
        bulletSpeed = speed;
        transform.up = moveDir;
        damage = d;
    }

    protected virtual void Update()
    {
        BulletTravel(bulletSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
            Destroy(gameObject);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    protected virtual void BulletTravel(float speed)
    {
        transform.position += (Vector3)(moveDir * speed * Time.deltaTime);
    }
}

// ---------------- Enemy Base ----------------

public abstract class EnemyEntity : MonoBehaviour
{
    public static event Action<EnemyEntity> OnAnyEnemyKilled;

    protected float enemyHP;
    protected float currentHP;
    protected float movementSpeed;
    protected float stopDistance;
    protected Vector2 target;
    protected Score score;

    private void Awake()
    {
        score = GameObject.Find("GameManager").GetComponent<Score>();
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
        currentHP -= damage;

        if (currentHP <= 0)
        {
            score.AddScore(1000);
            OnAnyEnemyKilled?.Invoke(this);
            Death();
        }
    }

    public virtual void Death()
    {
        Destroy(gameObject);
    }
}
