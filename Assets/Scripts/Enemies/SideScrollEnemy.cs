using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideScrollEnemy : EnemyEntity
{
    // Start is called before the first frame update
    void Start()
    {
        enemyHP = 20f;
        currentHP = enemyHP;
        attackSpeed = 2f;
        movementSpeed = 5f;
        stopDistance = 0;
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
}
