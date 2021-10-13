using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Linq;

public class Game : MonoBehaviour {

    public GameObject player_prefab;
    public GameObject projectile_prefab;
    public GameObject boostpacket_prefab;
    public GameObject start_state_object;

    public GameObject arena;
    public int players_count;

    public Material[] materials;
    public Material[] boostpacket_materials;

    [HideInInspector]
    public GameObject[] players;

    [HideInInspector]
    public Dictionary<int,GameObject> projectiles = new Dictionary<int,GameObject>(); 

    [HideInInspector]
    public Dictionary<int,GameObject> boostpackets = new Dictionary<int,GameObject>(); 

    [HideInInspector]
    public GameObject occluders;

    public GameObject[] states;

    [HideInInspector]
    public IState current_state;

    [HideInInspector]
    public bool game_running;

    public float music_start_time;

    public void Awake(){
        players = new GameObject[players_count];
    }

    public void start(){
        current_state = start_state_object.GetComponent<IState>();
        current_state.enter();
        arena = Instantiate(arena,Vector3.zero, Quaternion.identity);
    }

    public void Update()
    {
        if(Client.id != -1 && players[Client.id] != null){
            current_state.execute(); 
        }
    }


    public void changeStateTo(GameObject state){
        if (current_state != null)
            current_state.exit();

        current_state = state.GetComponent<IState>();
        current_state.enter();
    }

    public StateID getGameStateID(){
        return current_state.getStateID();
    }




    // Create player at random free position
    public void createPlayer(int id, string name,Vector3 position) {
        GameObject player = Instantiate(player_prefab, new Vector3(),  Quaternion.identity);
        GameObject player_object = (GameObject)player.transform.GetChild(0).transform.gameObject;
        player_object.transform.position = position;
        player_object.GetComponent<PlayerPosition>().target_position = position;
        player_object.GetComponent<PlayerAttributes>().id = id;
        player_object.GetComponent<PlayerAttributes>().name = name;
        player_object.transform.GetChild(0).transform.GetChild(0).GetComponent<Renderer>().material = materials[id];
        player_object.transform.GetChild(0).transform.GetChild(1).GetComponent<Renderer>().material = materials[id];
        players[id] = player;

        float time_dif = System.DateTime.Now.Second - music_start_time;
        float dif = time_dif/(float)0.90909090909;
        int m = (int)(dif);
        float d = dif - m;
        StartCoroutine(startPlayerAnim((1 - d) * (float)0.90909090909, player_object));
    }

    IEnumerator startPlayerAnim(float delay,GameObject player_object)
    {
        yield return new WaitForSeconds(delay);
        player_object.transform.GetChild(0).transform.GetComponent<Animator>().Play("PlayerAnimation");
    }

    public void removePlayer(int id){
        Destroy(players[id]);
        players[id] = null;
    }

    // destroys all corpses objects (only visual, no client notification neccessary)
    public void destroyAllCorpses(){
        foreach(GameObject corpse in GameObject.FindGameObjectsWithTag("PlayerCorpse")){
            Destroy(corpse);
        }
    }


    public void createProjectile(int projectile_id,int player_id, Vector3 position){
        GameObject projectile = Instantiate(projectile_prefab, position, Quaternion.identity);
        projectile.GetComponent<ProjectilePosition>().target_position = position;
        projectile.GetComponent<ProjectileAttributes>().player_id = player_id;
        projectile.GetComponent<ProjectileAttributes>().projectile_id = projectile_id;
        projectile.GetComponent<Renderer>().material = materials[player_id];
        projectile.transform.GetChild(0).transform.GetComponent<ParticleSystemRenderer>().material =  materials[player_id];
        projectiles.Add(projectile_id,projectile);
    }

    public void removeProjectile(int id){
        projectiles[id].GetComponent<ProjectileDeleteAnimation>().beginDestroyAnimation();
        Destroy(projectiles[id]);
        projectiles.Remove(id);
    }


    public void createBoostpacket(int id, int type){
        GameObject boostpacket = Instantiate(boostpacket_prefab,new Vector3(0,100,0), Quaternion.identity);
        boostpacket.GetComponent<BoostpacketPosition>().target_position = new Vector3(0,100,0);
        boostpacket.GetComponent<BoostpacketAttributes>().id = id;
        boostpacket.GetComponent<BoostpacketAttributes>().type = type;
        boostpacket.GetComponent<MeshRenderer>().material = boostpacket_materials[type];
        Debug.Log(boostpacket.transform.position);
        boostpackets.Add(id,boostpacket);
    }

    public void removeBoostpacket(int id){
        Destroy(boostpackets[id]);
        boostpackets.Remove(id);
    }

}

