using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : EnemyBulletSuperClass
{
    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
