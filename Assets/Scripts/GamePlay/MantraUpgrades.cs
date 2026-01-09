using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MantraUpgrades : MonoBehaviour
{
    PlayerMovement pM;
    GameObject player;



    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        pM = player.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }



}
