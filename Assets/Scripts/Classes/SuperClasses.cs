using UnityEngine;

//Darren Scott

public class EnemyBulletSuperClass : MonoBehaviour
{
    [SerializeField] private float lifeTime = 8f;

    private Vector2 moveDir = Vector2.down;
    protected float bulletSpeed = 8f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        // Optional: auto-despawn so bullets don't live forever
        CancelInvoke();
        if (lifeTime > 0f)
            Invoke(nameof(Despawn), lifeTime);
    }

    // This matches the pattern shooter usage: bullet.Fire(dir, speed)
    public virtual void Fire(Vector2 dir, float speed)
    {
        moveDir = dir.normalized;
        bulletSpeed = speed;

        if (rb != null)
        {
            rb.velocity = moveDir * bulletSpeed;
        }
    }

    protected virtual void Update()
    {
        // If no Rigidbody2D, move by transform as a fallback
        if (rb == null)
            transform.position += (Vector3)(moveDir * bulletSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
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


public abstract  class EnemyEntity : MonoBehaviour
{
    protected float enemyHP;
    protected float currentHP;
    protected float attackSpeed;
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

        if(Vector2.Distance(pos, target) <= stopDistance)
        {
            transform.position = target;
            return; 
        }
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage; 

        if(currentHP <= 0)
        {
            score.AddScore(1000);
            Destroy(gameObject);
        }
    }
}


