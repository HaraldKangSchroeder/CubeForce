using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerBoostAnimation : MonoBehaviour
{
    public GameObject on_fire_object;
    public GameObject on_speed_object;

    private PlayerAttributes player_attributes;

    private void Awake() {
        player_attributes = GetComponent<PlayerAttributes>();    
    }

    void Update(){
        on_fire_object.SetActive(player_attributes.on_fire);
        on_speed_object.SetActive(player_attributes.on_speed);
    }

}