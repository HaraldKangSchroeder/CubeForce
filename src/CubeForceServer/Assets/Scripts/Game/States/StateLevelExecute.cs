using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StateLevelExecute : MonoBehaviour, IState
{
    public GameObject next_state_object;
    public StateID state_id;

    // internal state variables
    private bool finished = false;

    private Coroutine coroutine;

    private Game game;
    private Server server;
    private GameAttributes game_attributes;

    public void Awake(){
        game = GameObject.Find("Game").GetComponent<Game>();
        server = GameObject.Find("Server").GetComponent<Server>();
        game_attributes = GameObject.Find("Game").GetComponent<GameAttributes>();
    }

    public void enter(){
        Debug.Log("Enter state : " + state_id);
        // start boost packet spawning
        coroutine = StartCoroutine(spawnBoostPacketWithDelay(game_attributes.boostpacket_delay));
    }

    public void execute(){
        // check if there are 1 or less players alive -> level 0 over
        int players_alive_count = 0;
        int players_count = 0;
        for(int i = 0; i < game.players.Length; i++){
            if(game.players[i] != null){
                players_count++;
                if(game.players[i].GetComponent<PlayerAttributes>().alive){
                    players_alive_count++;
                }
            }
        }
        finished = players_alive_count <= 1 && players_count > 1;

        // debug
        if(Input.GetKeyDown("k")){ // testwise press k
            finished = true;
        }
    }

    public void exit(){
        Debug.Log("Exit state : " + state_id);
        finished = false;
        StopCoroutine(coroutine);
    }

    public IState getNextState(){
        return next_state_object.GetComponent<IState>();
    }

    public StateID getStateID(){
        return state_id;
    }

    public bool isFinished(){
        return finished;
    }



    private IEnumerator spawnBoostPacketWithDelay(float delay){
        while(game.boostpackets.Count >= game_attributes.boostpacket_max_count){ // check if there are 4 or more boost packets already existent
            yield return new WaitForSeconds(2); // wait 2 seconds and check again    
        }
        yield return new WaitForSeconds(delay);
        game.createBoostPacket();

        coroutine = StartCoroutine(spawnBoostPacketWithDelay(game_attributes.boostpacket_delay));
    }


    public void handlePlayerKey(int id,bool key_state, PlayerKeys player_key){
        PlayerAttributes player_attributes = game.players[id].GetComponent<PlayerAttributes>();
        switch (player_key)
        {
            case PlayerKeys.MOVE_UP:
                player_attributes.key_move_up = key_state;
                break;
            case PlayerKeys.MOVE_LEFT:
                player_attributes.key_move_left = key_state;
                break;
            case PlayerKeys.MOVE_DOWN:
                player_attributes.key_move_down = key_state;
                break;
            case PlayerKeys.MOVE_RIGHT:
                player_attributes.key_move_right = key_state;
                break;
            case PlayerKeys.SHIELD:
                player_attributes.key_shield = key_state;
                break;
            case PlayerKeys.DODGE:
                player_attributes.key_dodge = key_state;
                break;
            case PlayerKeys.SHOOT:
                player_attributes.key_shoot = key_state;
                break;
        }
    }

    public void handlePlayerDirection(int id,float mouse_angle){
        GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerAttributes>().rotation_angle = mouse_angle;
    }

    public void finishState(){
        finished = true;
    }

}
