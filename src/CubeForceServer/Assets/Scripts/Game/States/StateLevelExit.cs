using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateLevelExit : MonoBehaviour, IState
{
    public GameObject next_state_object;
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
        StartCoroutine(FinishWithDelay(game_attributes.exit_delay));
    }

    public void execute(){

    }

    public void exit(){
        Debug.Log("Exit state : " + state_id);

        for(int i = 0; i < game.players.Length; i++){
            if(game.players[i] != null){
                GameObject player = game.players[i];
                PlayerAttributes player_attributes = player.GetComponent<PlayerAttributes>();
                player_attributes.resetKeys();
                if(player_attributes.alive){
                    player_attributes.wins += 1;

                    Packet packet = PacketManufacturer.playerWins(player_attributes.id);
                    server.sendMessageBroadcastReliable(packet);
                }
            }
        }

        // reset attributes
        finished = false;
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



    private IEnumerator FinishWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        finished = true;
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
