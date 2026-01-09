using UnityEngine;

//Darren Scott

public class EnemyBulletSuperClass : MonoBehaviour
{
    protected static float bulletSpeed;
    protected Vector2 moveDir = Vector2.down;

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

public class EnemyEntity : MonoBehaviour
{
    protected static float enemyHP;
    protected static float attackSpeed;
    protected static float movementSpeed;
    [SerializeField] static Vector2 target; 

    protected virtual void Movement(float speed)
    {
        float distance = Vector2.Distance(gameObject.transform.position, target);
        Vector2.MoveTowards(transform.position, target, distance);
    }
}
