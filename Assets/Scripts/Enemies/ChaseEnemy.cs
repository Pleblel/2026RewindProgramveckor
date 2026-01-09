using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ChaseMovement : EnemyEntity
{
    [SerializeField] GameObject playerTarget;
    [SerializeField] bool reachedPosition;
    bool hasBeenVisible;
    [SerializeField] GameObject runawayTarget;
    [SerializeField] float stayTimer = 2f; 

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
        Movement(movementSpeed);

        if (reachedPosition)
        {
            stayTimer -= Time.deltaTime;

            if (stayTimer <= 0f)
                RunAway(movementSpeed);
        }
           
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
            reachedPosition = true; 
            return;
        }
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }


    void RunAway(float speed)
    {

        Vector2 pos = transform.position;

        target = runawayTarget.transform.position;

        if (runawayTarget == null)
            return; 

       transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }
    void FindPlayer()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player"); 
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
