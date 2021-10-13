using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ProjectileCollision : MonoBehaviour
{
    private Rigidbody rb;
    private Game game;
    private Server server;
    private GameAttributes game_attributes;

    private ProjectileAttributes projectile_attributes;

    public void Awake(){
        projectile_attributes = GetComponent<ProjectileAttributes>();
        game = GameObject.Find("Game").GetComponent<Game>();
        server = GameObject.Find("Server").GetComponent<Server>();
        game_attributes = GameObject.Find("Game").GetComponent<GameAttributes>();
        rb = transform.GetComponent<Rigidbody>();
    }
    
    // reflect the projectile with respect to the normal at the collision point
    void OnCollisionEnter(Collision collision){
        rb.angularVelocity = new Vector3(0,0,0);
        Vector3 latest_velocity = GetComponent<ProjectilePosition>().latest_velocity;
        if(collision.gameObject.CompareTag("Player")){
            PlayerAttributes player_attributes_collision = collision.gameObject.GetComponent<PlayerAttributes>();
            if(player_attributes_collision.shield_active){
                if(player_attributes_collision.id != projectile_attributes.player_attributes.id){
                    game.removeProjectile(projectile_attributes.id);
                    // deactive shield of enemy player
                    collision.gameObject.transform.GetChild(1).gameObject.SetActive(false);
                    collision.gameObject.GetComponent<PlayerAttributes>().shield_active = false;

                    Packet packet = PacketManufacturer.playerShieldActive(player_attributes_collision.id);
                    server.sendMessageBroadcastReliable(packet);

                    // create new projectile of the hit player
                    Vector3 reflection_direction = Vector3. Reflect(latest_velocity, collision.contacts[0].normal).normalized;
                    game.createProjectile(player_attributes_collision.id,transform.position,reflection_direction, projectile_attributes.projectile_speed);
                }
                else{
                    projectile_attributes.projectile_speed = game_attributes.projectile_speed_increased;
                    rb.velocity = Vector3.Reflect(latest_velocity, collision.contacts[0].normal).normalized * projectile_attributes.projectile_speed;
                }
            }
            else if(collision.gameObject.GetComponent<PlayerAttributes>().id != projectile_attributes.player_attributes.id){
                Vector3 current_player_position = new Vector3(collision.gameObject.transform.position.x,1,collision.gameObject.transform.position.z);

                collision.gameObject.GetComponent<PlayerAttributes>().alive = false;

                // send everyone the information that the respective player died
                Packet packet = PacketManufacturer.playerAlive(collision.gameObject.GetComponent<PlayerAttributes>().id);
                server.sendMessageBroadcastReliable(packet);

                game.removeProjectile(projectile_attributes.id);

                // hide player somehow so that he is out of the game
                collision.gameObject.transform.position = new Vector3(current_player_position.x, 100, current_player_position.z);


                // add 1 kill to the player who has shot the projectile
                projectile_attributes.player_attributes.kills += 1;

                // inform all players about the amount of kills that the respective player currently has
                packet = PacketManufacturer.playerKills(projectile_attributes.player_attributes.id);
                server.sendMessageBroadcastReliable(packet);
                
            }
            else{
                projectile_attributes.projectile_speed = game_attributes.projectile_speed_increased;
                rb.velocity = Vector3.Reflect(latest_velocity, collision.contacts[0].normal).normalized * projectile_attributes.projectile_speed;
            }
        }
        else if(collision.gameObject.CompareTag("Projectile")){
            ProjectileAttributes projectile_attributes_collision = collision.gameObject.GetComponent<ProjectileAttributes>();
            if(projectile_attributes.player_attributes.id != projectile_attributes_collision.player_attributes.id)
            {
                game.removeProjectile(projectile_attributes.id);
                game.removeProjectile(projectile_attributes_collision.id);
            }
        }
        else{
            if(projectile_attributes.num_wall_collisions == 0){
                game.removeProjectile(projectile_attributes.id);
            }
            else{
                rb.velocity = Vector3.Reflect(latest_velocity, collision.contacts[0].normal).normalized * projectile_attributes.projectile_speed;
                projectile_attributes.num_wall_collisions -= 1;
            }
        }
    }

    

}
