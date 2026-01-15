using UnityEngine;

public class Bullet : PlayerBulletSuperClass
{
    [SerializeField] private float turnDegPerSec = 30f;

    protected override void Update()
    {
        // keep base movement + collision logic (collision is still in base)
        base.Update();
    }
}
