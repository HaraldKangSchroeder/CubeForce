using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPosition : MonoBehaviour
{
    private PlayerAttributes player_attributes;
    private Rigidbody rb;
    private Vector3 FORWARD_VECTOR = new Vector3(0,0,1f);
    private Vector3 RIGHT_VECTOR   = new Vector3(1f,0,0);
    private Server server;
    private GameAttributes game_attributes;

    void Start()
    {
        player_attributes = transform.GetComponent<PlayerAttributes>();
        server = GameObject.Find("Server").GetComponent<Server>();
        game_attributes = GameObject.Find("Game").GetComponent<GameAttributes>();
        rb = GetComponent<Rigidbody>();

        //StartCoroutine("send");
    }

    private void FixedUpdate() 
    {
        Vector3 direction = Vector3.zero;
        if(player_attributes.key_move_up){
            direction += FORWARD_VECTOR;
        }
        if(player_attributes.key_move_left){
            direction -= RIGHT_VECTOR;
        }
        if(player_attributes.key_move_down){
            direction -= FORWARD_VECTOR;
        }
        if(player_attributes.key_move_right){
            direction += RIGHT_VECTOR;
        }


        if(player_attributes.key_dodge && player_attributes.sidestep_ready)
        {
            StartCoroutine(sidestepDuration(game_attributes.sidestep_duration));
            StartCoroutine(sidestepReloadDuration(game_attributes.sidestep_cooldown_duration));
        }
        player_attributes.key_dodge = false;
        
        float fac = 1;
        if(player_attributes.sidestep_active) fac *= game_attributes.sidestep_speed_factor;
        if(player_attributes.on_speed) fac *= game_attributes.on_speed_factor;
        rb.velocity = direction.normalized * game_attributes.movement_speed * fac;
        
        
        // somehow its also necessary to set the angularVelocity to 0, else it might still move although the velocity itself is 0 already
        if(direction.magnitude == 0){
            rb.angularVelocity = Vector3.zero;
        }
    }

    private IEnumerator send()
    {
        Packet packet_player_position = PacketManufacturer.playerPosition(player_attributes.id);
        server.sendMessageBroadcast(packet_player_position);
        yield return new WaitForSeconds(server.send_interval);
        StartCoroutine("send");
    }

    IEnumerator sidestepDuration(float duration)
    {
        player_attributes.sidestep_active = true;
        yield return new WaitForSeconds(duration);
        player_attributes.sidestep_active = false;
    }

    IEnumerator sidestepReloadDuration(float duration)
    {
        player_attributes.sidestep_ready = false;

        // inform respective player than his sidestep is on cd now
        Packet packet = PacketManufacturer.playerSideStepReady(player_attributes.id);
        server.sendMessageReliable(player_attributes.id,packet);

        yield return new WaitForSeconds(duration/4f);
        player_attributes.sidestep_reloading_step = 1;
        yield return new WaitForSeconds(duration/4f);
        player_attributes.sidestep_reloading_step = 2;
        yield return new WaitForSeconds(duration/4f);
        player_attributes.sidestep_reloading_step = 3;
        yield return new WaitForSeconds(duration/4f);
        player_attributes.sidestep_reloading_step = 0;

        player_attributes.sidestep_ready = true;
        // inform respective player than his sidestep ready back again
        packet = PacketManufacturer.playerSideStepReady(player_attributes.id);
        server.sendMessageReliable(player_attributes.id,packet);
    }
}
