using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryShootingEnemy : EnemyEntity
{
    [SerializeField] GameObject targetGameOBJ;


    // Start is called before the first frame update
    void Start()
    {
        enemyHP = 100f;
        attackSpeed = 2f;
        movementSpeed = 5f;
        stopDistance = 0.05f;
        target = targetGameOBJ.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)
        Movement(movementSpeed);
    }
}
