using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerShooting : MonoBehaviour
{
    private PlayerAttributes player_attributes;


    private bool coroutine_running = false;

    private Game game;
    private GameAttributes game_attributes;
    private void Awake(){
        game = GameObject.Find("Game").GetComponent<Game>();
        game_attributes = GameObject.Find("Game").GetComponent<GameAttributes>();
    }

    void Start(){
        player_attributes = transform.GetComponent<PlayerAttributes>();
    }

    // Update is called once per frame
    void Update()
    {
        if(player_attributes.alive){
            if (player_attributes.key_shoot && player_attributes.munition > 0){

                Vector3 projectile_start_direction = (-1 * transform.right).normalized;
                Vector3 projectile_start_position = transform.position + projectile_start_direction * 1.5f;

                game.createProjectile(player_attributes.id,projectile_start_position,projectile_start_direction, player_attributes.on_fire ? game_attributes.projectile_speed_increased : game_attributes.projectile_speed_default);
                
                // decrease current munition
                player_attributes.munition -= 1;

                // send respective player his current munition
                Packet packet = PacketManufacturer.playerMunition(player_attributes.id);
                GameObject.Find("Server").GetComponent<Server>().sendMessageReliable(player_attributes.id,packet);

                if(!coroutine_running){
                    StartCoroutine(startReloading(game_attributes.munition_reloading_duration));
                }
            }
            player_attributes.key_shoot = false;
        }
    }



    IEnumerator startReloading(float reloading_duration)
    {
        coroutine_running = true;
        while(player_attributes.munition < game_attributes.munition_max){
            yield return new WaitForSeconds(reloading_duration/4f);
            player_attributes.munition_reloading_step = 1;
            yield return new WaitForSeconds(reloading_duration/4f);
            player_attributes.munition_reloading_step = 2;
            yield return new WaitForSeconds(reloading_duration/4f);
            player_attributes.munition_reloading_step = 3;
            yield return new WaitForSeconds(reloading_duration/4f);
            player_attributes.munition_reloading_step = 0;
            if(player_attributes.munition < game_attributes.munition_max){
                player_attributes.munition += 1;
                Packet packet = PacketManufacturer.playerMunition(player_attributes.id);
                GameObject.Find("Server").GetComponent<Server>().sendMessageReliable(player_attributes.id,packet);
            }
        }
        coroutine_running = false;
        yield return null;
    }
}
