using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    PlayerAttributes player_attributes;
    void Start()
    {
        player_attributes = GetComponent<PlayerAttributes>();
    }

    // Update is called once per frame
    // void Update()
    // {
    //     if(Client.id == player_attributes.id){
    //         if(Input.GetKeyDown("space")){

    //         }
    //         if(Input.GetKeyUp("space")){

    //         }
    //     }
    // }
}
