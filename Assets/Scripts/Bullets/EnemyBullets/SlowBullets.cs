using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowBullets : EnemyBulletSuperClass
{
    private void Update()
    {
        bulletSpeed = 5f;
        BulletTravel(bulletSpeed);
    }
}
