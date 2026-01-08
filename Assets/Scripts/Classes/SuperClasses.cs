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
