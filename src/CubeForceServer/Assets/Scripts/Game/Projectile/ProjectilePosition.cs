using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePosition : MonoBehaviour
{
    public Vector3 latest_velocity;
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

    private void Start() {
        latest_velocity = rb.velocity;
        //StartCoroutine("send");
    }

    void Update()
    {
        // set forward to its moving direction (to let the particles appear behind the projectile)
        transform.forward = rb.velocity.normalized;
    }

    void FixedUpdate()
    {
        // keep the velocity equal to the given projectile speed
        float projectile_speed = projectile_attributes.projectile_speed;
        if(rb.velocity.magnitude != projectile_speed){
            rb.velocity = rb.velocity.normalized * projectile_speed;
        }
        // the latest velocity is somehow important, else it might take a wrong vector during the collision computation (??? lol ok)
        latest_velocity = rb.velocity;
    }

    private IEnumerator send(){
        Packet packet_projectile_position = PacketManufacturer.projectilePosition(projectile_attributes.id);
        server.sendMessageBroadcast(packet_projectile_position);
        yield return new WaitForSeconds(server.send_interval);
        StartCoroutine("send");
    }
}
