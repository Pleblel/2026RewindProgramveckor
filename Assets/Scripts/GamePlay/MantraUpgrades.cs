using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MantraUpgrades : MonoBehaviour
{
    PlayerMovement pM;
    GameObject player;
    GameObject GM;
    Score scoreHandler;



    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        GM = GameObject.Find("GameManager");
        pM = player.GetComponent<PlayerMovement>();
        scoreHandler = GM.GetComponent<Score>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void AddSpeedMantra()
    {
        pM.normalSpeed *= 1.1f;
        scoreHandler.scoreMultiplier += 0.1f;
    }


}
