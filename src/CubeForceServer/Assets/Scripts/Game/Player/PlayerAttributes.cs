using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttributes : MonoBehaviour {

    public GameObject explosion_cube;
    public Material[] materials;

    [HideInInspector] public int munition;
    [HideInInspector] public string player_name;
    [HideInInspector] public int id;
    [HideInInspector] public bool sidestep_ready = true;
    [HideInInspector] public bool sidestep_active = false;
    [HideInInspector] public int sidestep_reloading_step;
    [HideInInspector] public bool shield_ready = true;
    [HideInInspector] public bool shield_active = false;
    [HideInInspector] public bool key_move_up;
    [HideInInspector] public bool key_move_left;
    [HideInInspector] public bool key_move_down;
    [HideInInspector] public bool key_move_right;
    [HideInInspector] public bool key_dodge;
    [HideInInspector] public bool key_shoot;
    [HideInInspector] public bool key_shield;
    [HideInInspector] public float rotation_angle;
    [HideInInspector] public int munition_reloading_step;
    [HideInInspector] public int kills;
    [HideInInspector] public int wins;
    [HideInInspector] public bool on_fire;
    [HideInInspector] public bool on_speed;
    [HideInInspector] public bool alive = true;
    [HideInInspector] public bool ready = false;


    private GameAttributes game_attributes;
    private Server server;

    private void Awake() {
        game_attributes = GameObject.Find("Game").GetComponent<GameAttributes>();
        server = GameObject.Find("Server").GetComponent<Server>();
    }


    public void startOnFire(){
        if(coroutine_on_fire != null) StopCoroutine(coroutine_on_fire);
        coroutine_on_fire = StartCoroutine(stopOnFireAfterSeconds(game_attributes.on_fire_duration));
    }

    Coroutine coroutine_on_fire;
    IEnumerator stopOnFireAfterSeconds(float delay)
    {
        on_fire = true;
        yield return new WaitForSeconds(delay);
        on_fire = false;
        Packet packet = PacketManufacturer.playerOnFire(id);
        server.sendMessageBroadcastReliable(packet);
    }

    Coroutine coroutine_on_speed;
    public void startOnSpeed(){
        if(coroutine_on_speed != null) StopCoroutine(coroutine_on_speed);
        coroutine_on_speed = StartCoroutine(stopOnSpeedAfterSeconds(game_attributes.on_speed_duration));
    }

    IEnumerator stopOnSpeedAfterSeconds(float delay)
    {
        on_speed = true;
        yield return new WaitForSeconds(delay);
        on_speed = false;
        Packet packet = PacketManufacturer.playerOnSpeed(id);
        server.sendMessageBroadcastReliable(packet);
    }

    public void resetKeys(){
        key_move_up = false;
        key_move_left = false;
        key_move_down = false;
        key_move_right = false;
        key_dodge = false;
        key_shoot = false;
        key_shield = false;
    }

}
