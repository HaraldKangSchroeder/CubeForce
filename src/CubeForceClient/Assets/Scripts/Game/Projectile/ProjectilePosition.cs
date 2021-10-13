using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePosition : MonoBehaviour
{
    // ProjectileAttributes projectile_attributes;

    // public Vector3 target_position;

    // private Coroutine coroutine;
    // private bool coroutine_running;

    // // Start is called before the first frame update
    // void Start()
    // {
    //     projectile_attributes = GetComponent<ProjectileAttributes>();
    // }

    // public void updatePosition(Vector3 position){
    //     if(coroutine_running){
    //         StopCoroutine(coroutine);
    //         coroutine_running = false;
    //         transform.position = target_position;
    //     }

    //     target_position = position;

    //     coroutine = StartCoroutine("moveToTargetPosition");
    // }

    // private IEnumerator moveToTargetPosition(){
    //     coroutine_running = true;
    //     Vector3 position_delta = (target_position - transform.position)/2f;
    //     transform.position += position_delta;

    //     yield return new WaitForSeconds(Client.server_message_frequency/2f);
    //     transform.position += position_delta;

    //     coroutine_running = false;
    // }

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

        update_step =  GameObject.Find("Game").GetComponent<UpdateFrequencyTracker>().getAverageUpdateSteps();// getAverageUpdateSteps();
        direction_step = (target_position - transform.position)/update_step;
    }
}
