using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : PlayerBulletSuperClass
{
    Vector2 spread;
    private void Update()
    {
        bulletSpeed = 5f;

        BulletTravel(bulletSpeed);
        
    }
}
