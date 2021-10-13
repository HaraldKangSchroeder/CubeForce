using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAttributes : MonoBehaviour
{
    [Header("Game")]
    public GameObject arena;
    public GameObject player;
    public GameObject projectile;
    public GameObject[] boostpackets;
    public Vector2[] start_positions;

    [Header("Player")]
    public float movement_speed;
    public float on_fire_duration;
    public float on_speed_duration;
    public float on_speed_factor;
    public float sidestep_duration;
    public float sidestep_cooldown_duration;
    public float sidestep_speed_factor;
    public float shield_duration;
    public float shield_cooldown_duration;
    public int munition_max;
    public int munition_boost;
    public float munition_reloading_duration;


    [Header("Projectile")]
    public float projectile_speed_default;
    public float projectile_speed_increased;

    [Header("Boostpacket")]
    public Vector2 boostpacket_spawning_range;
    public float boostpacket_delay;
    public int boostpacket_max_count;

    [Header("State")]
    public float enter_delay;
    public float transition_delay;
    public float exit_delay;
}
