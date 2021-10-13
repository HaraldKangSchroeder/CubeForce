using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateTransition : MonoBehaviour, IState
{
    public GameObject next_state_object;
    public StateID state_id;
    private bool finished = false;
    private Game game;
    private GameAttributes game_attributes;

    public void Awake(){
        game = GameObject.Find("Game").GetComponent<Game>();
        game_attributes = GameObject.Find("Game").GetComponent<GameAttributes>();
    }

    public void enter(){
        Debug.Log("Enter state : " + state_id);
        // present lobby ui that visualizes all connected players

        // remove remaining components before entering the next state
        game.removeAllProjectiles();
        game.removeAllBoostpackets();
        // destroyAllCorpses();
        // remove arena object of lvl 0
        // move occluders and player that is alive (if existent) updwards. delete occluders afterwards;
        game.occluders.GetComponent<Animator>().Play("OccludersFadeOut");
        StartCoroutine(FinishWithDelay(game_attributes.transition_delay));
        

    }

    public void execute(){

        for(int i = 0; i < game.players.Length; i++){
            if(game.players[i] != null && game.players[i].GetComponent<PlayerAttributes>().alive){
                game.players[i].transform.position = new Vector3(game.players[i].transform.position.x,game.occluders.transform.position.y + 2,game.players[i].transform.position.z);
            }
        }

        // check if a specific event occured which indicates a following state exit
        if(Input.GetKeyDown("k")){ // testwise press k
            finished = true;
        }
    }

    public void exit(){
        Debug.Log("Exit state : " + state_id);
        // remove lobby ui
        Destroy(game.occluders);
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

    }

    public void handlePlayerDirection(int id,float mouse_angle){
        
    }

    public void finishState(){
        finished = true;
    }


}
