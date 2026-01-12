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
}
