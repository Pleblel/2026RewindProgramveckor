using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ChaseMovement : EnemyEntity
{
    //Darren Scott


    [SerializeField] GameObject playerTarget;
    [SerializeField] bool reachedPosition;
    bool firedBullet = false;
    bool hasBeenVisible;
    [SerializeField] GameObject runawayTarget;
    [SerializeField] float stayTimer = 2f;
    [SerializeField] GameObject bullet;
    [SerializeField] float distanceStop;

    // Start is called before the first frame update
    void Start()
    {
        FindPlayer();
        distanceStop = Random.Range(2, 6);
        enemyHP = 10f;
        currentHP = enemyHP;
        movementSpeed = 5f;
        stopDistance = distanceStop;
        target = playerTarget.transform.position;

        if (DifficultyManager.I != null)
            movementSpeed *= DifficultyManager.I.MoveSpeedMult;
    }

    // Update is called once per frame
    void Update()
    {
        Movement(movementSpeed);

        if (reachedPosition)
        {
            stayTimer -= Time.deltaTime;

            if (!firedBullet)
            {
                FireBullet();
                firedBullet = true;
            }

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

    void FireBullet()
    {
        Instantiate(bullet, transform.position, Quaternion.identity);
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
