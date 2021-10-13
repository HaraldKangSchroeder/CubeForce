using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateLevelEnter : MonoBehaviour, IState
{
    public GameObject next_state_object;
    public GameObject occluders;
    public Vector2[] free_positions; //make sure to have enough
    public StateID state_id;
    private bool finished = false;

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
        game.occluders = Instantiate(occluders, new Vector3(0,35,0), Quaternion.identity);
        game.occluders.GetComponent<Animator>().Play("OccludersFadeIn");

        for(int i = 0; i < game.players.Length; i++){
            if(game.players[i] != null){
                GameObject player = game.players[i];
                PlayerAttributes player_attributes = player.GetComponent<PlayerAttributes>();
                
                // set everyone alive again and inform all players respectively
                resetPlayerAlive(player_attributes);
                resetPlayerMunition(player_attributes);
                resetPlayerShield(player_attributes);

                player.transform.position = new Vector3(free_positions[i].x,37,free_positions[i].y);
            }
        }
        StartCoroutine(FinishWithDelay(game_attributes.enter_delay));
    }

    private void resetPlayerAlive(PlayerAttributes player_attributes){
        player_attributes.alive = true;
        Packet packet = PacketManufacturer.playerAlive(player_attributes.id);
        server.sendMessageBroadcastReliable(packet);
    }

    private void resetPlayerMunition(PlayerAttributes player_attributes){
        player_attributes.munition = game_attributes.munition_max;
        Packet packet = PacketManufacturer.playerMunition(player_attributes.id);
        server.sendMessageReliable(player_attributes.id, packet);
    }

    private void resetPlayerShield(PlayerAttributes player_attributes){
        player_attributes.shield_ready = true;
        Packet packet = PacketManufacturer.playerShieldReady(player_attributes.id);
        server.sendMessageReliable(player_attributes.id,packet);
    }

    public void execute(){
        for(int i = 0; i < game.players.Length; i++){
            if(game.players[i] != null){
                game.players[i].transform.position = new Vector3(game.players[i].transform.position.x,game.occluders.transform.position.y + 2,game.players[i].transform.position.z);
            }
        }
    }

    public void exit(){
        Debug.Log("Exit state : " + state_id);
        finished = false;
    }

    private IEnumerator FinishWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        finished = true;
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
        GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerAttributes>().rotation_angle = mouse_angle;
    }

    public void finishState(){
        finished = true;
    }

}
