using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Darren Scott
public class NormalBullets : EnemyBulletSuperClass
{
    private void Update()
    {
        bulletSpeed = 10f;
        BulletTravel(bulletSpeed);
    }
}
