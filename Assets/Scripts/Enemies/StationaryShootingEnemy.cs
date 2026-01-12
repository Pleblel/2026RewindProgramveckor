using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryShootingEnemy : EnemyEntity
{
    [SerializeField] GameObject targetGameOBJ;
    private bool hasTarget = false; 

    //Darren Scott


    // Start is called before the first frame update
    void Start()
    {
        enemyHP = 100f;
        currentHP = enemyHP; 
        attackSpeed = 2f;
        movementSpeed = 25f;
        stopDistance = 0.05f;
        target = targetGameOBJ.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(hasTarget)
        Movement(movementSpeed);
    }

    public void FindTargetPosition(Transform seat)
    {
        target = seat.position;
        hasTarget = true; 
    }

}
