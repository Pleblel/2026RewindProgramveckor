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

public class EnemyEntity : MonoBehaviour
{
    protected static float enemyHP;
    protected static float attackSpeed;
    protected static float movementSpeed;
    protected static float stopDistance;
    protected static Vector2 target; 

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
}
