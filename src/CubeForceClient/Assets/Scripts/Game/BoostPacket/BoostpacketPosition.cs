using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostpacketPosition : MonoBehaviour
{
    public Vector3 target_position;
    private int update_step = 0;
    private Vector3 direction_step = Vector3.zero;


    void Update(){
        if(update_step > 0){
            update_step--;
            transform.position += direction_step;
        }
        else{
            transform.position += direction_step/2f;
        }
    }

    public void updatePosition(Vector3 position){
        //transform.position = target_position;
        target_position = position;

        update_step = GameObject.Find("Game").GetComponent<UpdateFrequencyTracker>().getAverageUpdateSteps();
        direction_step = (target_position - transform.position)/update_step;
    }
}
