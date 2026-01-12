using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Bullet : PlayerBulletSuperClass
{
    private void Update()
    {
        BulletTravel(bulletSpeed);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            var enemy = collision.GetComponent<EnemyEntity>();
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }

        
           
    }

}
