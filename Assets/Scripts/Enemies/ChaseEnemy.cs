using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ChaseMovement : EnemyEntity
{
    [SerializeField] GameObject playerTarget; 



    // Start is called before the first frame update
    void Start()
    {
        FindPlayer();
        enemyHP = 100f;
        attackSpeed = 2f;
        movementSpeed = 5f;
        stopDistance = 2f;
        target = playerTarget.transform.position; 
    }

    // Update is called once per frame
    void Update()
    {
        FindPlayer();
        Movement(movementSpeed);
    }

    protected override void Movement(float speed)
    {

        Vector2 pos = transform.position;

        if (playerTarget == null)
        {
            return;
        }

        if (Vector2.Distance(pos, target) <= stopDistance)
        {
            return;
        }
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    void FindPlayer()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player"); 
    }
}
