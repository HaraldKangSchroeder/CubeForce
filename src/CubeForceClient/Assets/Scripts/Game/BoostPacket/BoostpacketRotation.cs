using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostpacketRotation : MonoBehaviour
{
    public Quaternion target_rotation;
    private int update_step = 0;
    private float direction_factor = 1;

    void Update(){
        if(update_step > 0){
            update_step--;
            transform.rotation = Quaternion.Slerp(transform.rotation, target_rotation, direction_factor);
        }
        // due to floating point calculation errors?
        else{
            transform.rotation = target_rotation;
        }
    }





    public void updateRotation(Quaternion rotation){
        transform.rotation = target_rotation;
        target_rotation = rotation;

        update_step = GameObject.Find("Game").GetComponent<UpdateFrequencyTracker>().getAverageUpdateSteps();
        direction_factor = update_step != 0 ? 1f/update_step : 1f;
    }

}
