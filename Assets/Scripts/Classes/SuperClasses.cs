using UnityEngine;

//Darren Scott

public class EnemyBulletSuperClass : MonoBehaviour
{
    protected static float bulletSpeed;
    protected static Vector2 moveDir = Vector2.down;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            Destroy(gameObject);        
    }
    protected virtual void BulletTravel(float speed)
    {
        transform.position += (Vector3)(moveDir.normalized * speed * Time.deltaTime);
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


