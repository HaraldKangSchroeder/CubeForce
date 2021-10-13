using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttributes : MonoBehaviour
{

    public GameObject explosion_cube;

    public GameObject player_body;
    public GameObject player_head;

    public int id;
    public string name;
    public bool alive = true;

    public bool on_fire;
    public bool on_speed;

    public int max_munition;
    public int munition;
    public int munition_reloading_step;

    public bool sidestep_ready;
    public int sidestep_reloading_step;

    public bool shield_ready;
    public bool shield_active;

    public Vector3 death_position;
    public Vector3 death_rotation;
    public Vector3 death_direction;


    public float angle_to_mouse;

    public bool key_move_up;
    public bool key_move_left;
    public bool key_move_down;
    public bool key_move_right;

    public bool key_dodge;

    public int kills;

    public int wins;

    public bool ready = false;
}
