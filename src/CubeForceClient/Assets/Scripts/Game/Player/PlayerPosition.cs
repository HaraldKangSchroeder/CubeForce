using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerPosition : MonoBehaviour
{

    // class PositionEntry{
    //     public long time_milliseconds;
    //     public Vector3 position;

    //     public PositionEntry(Vector3 position,long time_milliseconds){
    //         this.position = position;
    //         this.time_milliseconds = time_milliseconds;
    //     }
    // }

    // private PlayerAttributes player_attributes;
    // private Rigidbody rb;

    // private float THRESHOLD = 1f;

    // private bool sidestep_active = false;

    // private Vector3 FORWARD_VECTOR = new Vector3(0,0,1f);
    // private Vector3 RIGHT_VECTOR   = new Vector3(1f,0,0);
    // private Vector3 ZERO_VECTOR   = new Vector3(0,0,0);

    // private List<PositionEntry> memory = new List<PositionEntry>();

    // void Start(){
    //     player_attributes = GetComponent<PlayerAttributes>();
    //     rb = GetComponent<Rigidbody>();
    // }

    // void FixedUpdate(){
    //     if(player_attributes.alive){
    //         refreshMemory();
    //         handleMovement();
    //     }
    // }



    // private void handleMovement(){
    //     Vector3 direction = ZERO_VECTOR;
    //     if(player_attributes.key_move_up){
    //         direction += FORWARD_VECTOR;
    //     }
    //     if(player_attributes.key_move_left){
    //         direction += RIGHT_VECTOR * -1;
    //     }
    //     if(player_attributes.key_move_down){
    //         direction += FORWARD_VECTOR * -1;
    //     }
    //     if(player_attributes.key_move_right){
    //         direction += RIGHT_VECTOR;
    //     }

    //     if(player_attributes.key_dodge && player_attributes.sidestep_ready){
    //         StartCoroutine(sidestepDuration(0.2f));
    //     }
    //     player_attributes.key_dodge = false;

    //     if(sidestep_active){
    //         rb.velocity = direction.normalized * 9 * 3.5f;
    //     }
    //     else{
    //         rb.velocity = direction.normalized * 9;
    //     }

    //     if(direction.magnitude == 0){
    //         rb.angularVelocity = ZERO_VECTOR;
    //     }  
    // }

    // IEnumerator sidestepDuration(float duration)
    // {
    //     Debug.Log("Enter side step duration coroutine");
    //     sidestep_active = true;
    //     yield return new WaitForSeconds(duration);
    //     sidestep_active = false;
    //     yield return null;
    // }

    // public void synchronizePosition(Vector3 position,long time_milliseconds){
    //     refreshMemory();
    //     // if(player_attributes.id == Client.id){
    //     //     time_milliseconds -= (long)(Client.getPingInMilliseconds()/2);        
    //     // }

    //     //time_milliseconds += (long)(Client.getPingInMilliseconds()/2);  

    //     int index = binarySearch(time_milliseconds,0,memory.Count);
    //     if(index == 0 && time_milliseconds < memory[index].time_milliseconds){
    //         resetMemory(position);
    //     }
    //     else{
    //         Vector3 position_interpolated;
    //         if(index == memory.Count - 1){
    //             Debug.Log("QUAL");
    //             position_interpolated = memory[index].position;
    //         }
    //         else{
    //             float a = (float)((double)(time_milliseconds - memory[index].time_milliseconds)/(double)(memory[index + 1].time_milliseconds - memory[index].time_milliseconds));
    //             //Debug.Log(a);
    //             position_interpolated = (1 - a) * memory[index].position + a * memory[index + 1].position;
    //         }

    //         // check if prediction was too wrong
    //         if((position_interpolated - position).magnitude > THRESHOLD){
    //             Debug.Log("Wrong prediction ID : " + player_attributes.id + " with error : " + (position_interpolated - position).magnitude);
    //             resetMemory(position);
    //         }
    //     }
    // }

    // private int binarySearch(long time_milliseconds, int left, int right){
    //     if(right - left <= 1){
    //         return left;
    //     }
    //     int index = left + (int)((right-left)/2);
    //     if(memory[index].time_milliseconds < time_milliseconds){
    //         return binarySearch(time_milliseconds,index,right);
    //     }
    //     else{
    //         return binarySearch(time_milliseconds,left,index);
    //     }
    // }

    // private void refreshMemory(){
    //     long time_milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
    //     if(memory.Count >= 200){
    //         memory.RemoveAt(0);
    //     }
    //     memory.Add(new PositionEntry(transform.position,time_milliseconds));
    // }

    // private void resetMemory(Vector3 position){
    //     memory.Clear();
    //     transform.position = position;
    //     memory.Add(new PositionEntry(transform.position, DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond));
    // }


    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // PlayerAttributes player_attributes;

    // public Vector3 target_position;

    // private Coroutine coroutine;
    // private bool coroutine_running;

    // // Start is called before the first frame update
    // void Start()
    // {
    //     player_attributes = GetComponent<PlayerAttributes>();
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

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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


    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // PlayerAttributes player_attributes;
    // Rigidbody rb;

    // public Vector3 target_latest;
    // public Vector3 target_predicted;

    // private float sidestep_speed_factor = 1f;

    // void Start(){
    //     player_attributes= GetComponent<PlayerAttributes>();
    //     rb = GetComponent<Rigidbody>();
    // }

    // public void Update(){
    //     Debug.Log("Update");
    // }

    // public void FixedUpdate(){
    //     Debug.Log("Fixed Update");
    //     Vector3 _direction = target_predicted - transform.position;
    //     if(_direction.magnitude > 5f){
    //         Debug.Log("too far away - directly set new position");
    //         transform.position = target_predicted;
    //         rb.velocity = Vector3.zero;
    //     }
    //     if(_direction.magnitude < 1f){
    //         Debug.Log("almost at predicted position, stop moving");
    //         rb.velocity = Vector3.zero;
    //         rb.angularVelocity = Vector3.zero;
    //     }
    //     else{
    //         if(player_attributes.key_dodge){
    //             player_attributes.key_dodge = false;
    //             if(player_attributes.sidestep_ready){
    //                 StartCoroutine(sidestepDuration(0.2f));
    //             }
    //         }
    //         rb.velocity = _direction.normalized * 9 * sidestep_speed_factor;
    //     }
        
    // }

    // public void updatePosition(Vector3 position){
    //     Debug.Log("Position update");
    //     Vector3 _position_change = position - target_latest; 
    //     target_latest = position;
    //     target_predicted = position + _position_change;
    //     //transform.position = transform.position + (target_latest - transform.position)/2f;
    // }

    // IEnumerator sidestepDuration(float duration)
    // {
    //     sidestep_speed_factor = 3.5f;
    //     yield return new WaitForSeconds(duration);
    //     sidestep_speed_factor = 1f;
    // }
}
