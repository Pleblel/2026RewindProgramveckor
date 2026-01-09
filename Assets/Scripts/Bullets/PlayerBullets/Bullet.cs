using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : PlayerBulletSuperClass
{
    private void Update()
    {
        bulletSpeed = 5f;
        BulletTravel(bulletSpeed);
    }
}
