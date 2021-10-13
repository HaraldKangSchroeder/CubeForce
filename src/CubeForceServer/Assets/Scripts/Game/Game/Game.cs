using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Linq;

public class Game : MonoBehaviour
{
    public GameObject start_state_object;
    public IState current_state;

    [HideInInspector] public GameObject occluders;
    [HideInInspector] public GameObject[] players;
    [HideInInspector] public Dictionary<int, GameObject> projectiles = new Dictionary<int, GameObject>();
    [HideInInspector] public Dictionary<int, GameObject> boostpackets = new Dictionary<int, GameObject>();

    private int projectiles_id_counter = 0;
    private int boostpackets_id_counter = 0;

    private Server server;
    private GameAttributes game_attributes;

    public void Awake()
    {
        players = new GameObject[Server.num_clients];
        server = GameObject.Find("Server").GetComponent<Server>();
        game_attributes = GetComponent<GameAttributes>();
    }

    public void Start()
    {
        current_state = start_state_object.GetComponent<IState>();
        current_state.enter();
        Instantiate(game_attributes.arena, Vector3.zero, Quaternion.identity);
        StartCoroutine("sendPlayersTransform");
        StartCoroutine("sendProjectilesTransform");
        StartCoroutine("sendBoostpacketsTransform");
    }

    public void Update()
    {
        // state machine
        if (current_state.isFinished())
        {
            changeState();
            Packet packet = PacketManufacturer.gameState((int)current_state.getStateID());
            server.sendMessageBroadcastReliable(packet);
        }
        else
        {
            current_state.execute();
        }
    }

    public void changeState()
    {
        if (current_state != null)
            current_state.exit();

        current_state = current_state.getNextState();
        current_state.enter();
    }

    public StateID getGameStateID()
    {
        return current_state.getStateID();
    }

    public void handlePlayerKey(int id, bool key_state, PlayerKeys player_key)
    {
        current_state.handlePlayerKey(id, key_state, player_key);
    }

    public void handlePlayerDirection(int id, float mouse_angle)
    {
        current_state.handlePlayerDirection(id, mouse_angle);
    }




    public void createPlayer(int id, string name)
    {
        Vector2 position2d = GetComponent<GameAttributes>().start_positions[id];
        GameObject player = Instantiate(game_attributes.player, new Vector3(position2d.x, 2, position2d.y), Quaternion.identity);
        player.GetComponent<PlayerAttributes>().id = id;
        player.GetComponent<PlayerAttributes>().player_name = name;
        players[id] = player;
    }

    public void removePlayer(int id)
    {
        if (players[id] != null)
        {
            Destroy(players[id]);
            players[id] = null;

            // tell all connected players that a player left the game
            // Packet packet = PacketManufacturer.playerLeave(id);
            // Server.sendMessageBroadcastReliable(packet);
        }
    }

    public void createProjectile(int player_id, Vector3 position, Vector3 direction, float projectile_speed)
    {
        GameObject projectile = Instantiate(game_attributes.projectile, position, Quaternion.identity);

        // assignt id of projectile and the id of the player who wants to shoot to the projectile
        ProjectileAttributes projectile_attributes = projectile.transform.GetComponent<ProjectileAttributes>();
        projectile_attributes.id = getFreeID(projectiles, ref projectiles_id_counter);
        projectile_attributes.num_wall_collisions = getNumWallCollisions();
        projectile_attributes.player_attributes = players[player_id].GetComponent<PlayerAttributes>();
        projectile.transform.forward = direction;
        projectile.transform.GetComponent<Rigidbody>().velocity = direction;
        projectile_attributes.projectile_speed = projectile_speed;
        projectiles.Add(projectile_attributes.id, projectile);

        Packet packet = PacketManufacturer.projectileJoin(projectile_attributes.id, player_id, position);
        server.sendMessageBroadcastReliable(packet);
    }

    public void removeProjectile(int id)
    {
        if (projectiles.ContainsKey(id))
        {
            Destroy(projectiles[id]);
            projectiles.Remove(id);

            Packet packet = PacketManufacturer.projectileLeave(id);
            server.sendMessageBroadcastReliable(packet);
        }
    }

    // destroys all existent projectiles in game and in interface so that the client gets the delete information as well
    public void removeAllProjectiles()
    {
        foreach (GameObject projectile in projectiles.Values)
        {
            int id = projectile.GetComponent<ProjectileAttributes>().id;
            Packet packet = PacketManufacturer.projectileLeave(id);
            server.sendMessageBroadcastReliable(packet);
            Destroy(projectile);
        }
        projectiles.Clear();
    }

    public void createBoostPacket()
    {
        float random_x = UnityEngine.Random.Range(-game_attributes.boostpacket_spawning_range.x, game_attributes.boostpacket_spawning_range.x);
        float random_z = UnityEngine.Random.Range(-game_attributes.boostpacket_spawning_range.y, game_attributes.boostpacket_spawning_range.y);
        Vector3 boostpacket_position = new Vector3(random_x, 30, random_z);
        Vector3 boostpacket_rotation = new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360));
        GameObject boostpacket = Instantiate(
                                                game_attributes.boostpackets[UnityEngine.Random.Range(0, game_attributes.boostpackets.Length)],
                                                boostpacket_position,
                                                Quaternion.Euler(boostpacket_rotation.x, boostpacket_rotation.y, boostpacket_rotation.z)
                                            );

        boostpacket.GetComponent<BoostpacketAttributes>().id = getFreeID(boostpackets, ref boostpackets_id_counter);
        boostpackets.Add(boostpacket.GetComponent<BoostpacketAttributes>().id, boostpacket);

        Packet packet = PacketManufacturer.boostpacketJoin(boostpacket.GetComponent<BoostpacketAttributes>().id);
        server.sendMessageBroadcastReliable(packet);
    }

    public void removeBoostPacket(int id)
    {
        if (boostpackets.ContainsKey(id))
        {
            Destroy(boostpackets[id]);
            boostpackets.Remove(id);

            Packet packet = PacketManufacturer.boostpacketLeave(id);
            server.sendMessageBroadcastReliable(packet);
        }
    }

    // destroys all existent projectiles in game and in interface so that the client gets the delete information as well
    public void removeAllBoostpackets()
    {
        foreach (GameObject boostpacket in boostpackets.Values)
        {
            int id = boostpacket.GetComponent<BoostpacketAttributes>().id;
            Packet packet = PacketManufacturer.boostpacketLeave(id);
            server.sendMessageBroadcastReliable(packet);
            Destroy(boostpacket);
        }
        boostpackets.Clear();
    }

    public int getFreeID(Dictionary<int, GameObject> dic, ref int id_counter)
    {
        for (int i = 0; i < 255; i++)
        {
            id_counter = (id_counter + 1) % 255;
            if (!dic.ContainsKey(id_counter))
            {
                return id_counter;
            }
        }
        return -1;
    }

    public int getNumWallCollisions()
    {
        int num_players_alive = 0;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] != null)
            {
                if (players[i].GetComponent<PlayerAttributes>().alive)
                    num_players_alive++;
            }
        }
        if (num_players_alive == 4) return 0;
        else if (num_players_alive == 3) return 1;
        else if (num_players_alive <= 2) return 2;
        else return 0;

    }



    // destroys all existent boost packets in game and in interface so that the client gets the delete information as well
    private void destroyAllBoostPackets()
    {
        foreach (GameObject boostpacket in boostpackets.Values)
        {
            Destroy(boostpacket);
        }
        boostpackets.Clear();
    }

    private IEnumerator sendPlayersTransform()
    {
        foreach (GameObject player in players) {
            if(player != null){
                Packet packet = PacketManufacturer.playerTransform(player.GetComponent<PlayerAttributes>().id);
                server.sendMessageBroadcast(packet);
            }
        }
        yield return new WaitForSeconds(server.send_interval);
        StartCoroutine("sendPlayersTransform");
    }

    private IEnumerator sendProjectilesTransform()
    {
        if(projectiles.Count > 0){
            Packet packet = PacketManufacturer.projectilesTransform();
            server.sendMessageBroadcast(packet);
        }
        yield return new WaitForSeconds(server.send_interval);
        StartCoroutine("sendProjectilesTransform");
    }


    private IEnumerator sendBoostpacketsTransform()
    {
        if(boostpackets.Count > 0){
            Packet packet = PacketManufacturer.boostpacketsTransform();
            server.sendMessageBroadcast(packet);
        }
        yield return new WaitForSeconds(server.send_interval);
        StartCoroutine("sendBoostpacketsTransform");
    }

    // // destroys all corpses objects (only visual, no client notification neccessary)
    // private void destroyAllCorpses(){
    //     foreach(GameObject corpse in GameObject.FindGameObjectsWithTag("PlayerCorpse")){
    //         Destroy(corpse);
    //     }
    // }

}
