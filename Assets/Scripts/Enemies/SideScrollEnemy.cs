using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SideScrollEnemy : EnemyEntity
{

    [SerializeField] Transform targetPoint;
    [SerializeField] GameObject bullet;
    bool hasBeenVisible = false;

    // Start is called before the first frame update
    void Start()
    {
        enemyHP = 20f;
        currentHP = enemyHP;
        movementSpeed = 10f;
        stopDistance = 0;
        target = targetPoint.position;
    }

    // Update is called once per frame
    void Update()
    {
        Movement(movementSpeed);
    }

    protected override void Movement(float speed)
    {
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    public override void Death()
    {
        Instantiate(bullet,transform.position,transform.rotation);
        Destroy(gameObject);
    }

    private void OnBecameVisible()
    {
        hasBeenVisible = true;
    }

    private void OnBecameInvisible()
    {
        if (hasBeenVisible)
            Destroy(gameObject);
    }
}
