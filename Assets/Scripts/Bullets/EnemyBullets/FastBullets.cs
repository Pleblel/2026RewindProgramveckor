using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Darren Scott
public class FastBullets : EnemyBulletSuperClass
{

    private void Update ()
    {
        bulletSpeed = 15f;
        BulletTravel(bulletSpeed);
    }

}
