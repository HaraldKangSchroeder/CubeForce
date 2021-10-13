using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateAwardCeremony : MonoBehaviour, IState
{
    public GameObject next_state_object;
    public StateID state_id;

    // internal state variables
    private bool finished = false;

    private Game game;
    private Server server;

    public void Awake(){
        game = GameObject.Find("Game").GetComponent<Game>();
        server = GameObject.Find("Server").GetComponent<Server>();
    }

    public void enter(){
        Debug.Log("Enter state : " + state_id);
        // present award ceremony ui that visualizes the ranking of this round
    }

    public void execute(){
        // check if a specific event occured which indicates a following state exit
        if(Input.GetKeyDown("k")){
            finished = true;
        }
    }

    public void exit(){
        Debug.Log("Exit state : " + state_id);
        // remove award ceremony ui

        // reset attributes
        finished = false;

        // reset wins and kills of all players
        for(int i = 0; i < game.players.Length; i++){
            if(game.players[i] != null){
                PlayerAttributes player_attributes = game.players[i].GetComponent<PlayerAttributes>();

                player_attributes.kills = 0;
                Packet packet = PacketManufacturer.playerKills(player_attributes.id);
                server.sendMessageBroadcastReliable(packet);

                player_attributes.wins = 0;
                packet = PacketManufacturer.playerWins(player_attributes.id);
                server.sendMessageBroadcastReliable(packet);
            }
        }
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

    public void handlePlayerKey(int id,bool key_state, PlayerKeys player_key){

    }

    public void handlePlayerDirection(int id,float mouse_angle){

    }

    public void finishState(){
        finished = true;
    }

}
