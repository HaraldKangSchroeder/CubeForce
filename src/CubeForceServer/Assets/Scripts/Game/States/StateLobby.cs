using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateLobby : MonoBehaviour, IState
{
    public GameObject next_state_object;
    public StateID state_id;


    // internal state variables
    private bool finished = false;


    private Coroutine coroutine;
    private float connection_check_frequency = 2;
    
    private Game game;
    private Server server;

    public void Awake(){
        game = GameObject.Find("Game").GetComponent<Game>();
        server = GameObject.Find("Server").GetComponent<Server>();
    }

    public void enter(){
        Debug.Log("Enter state : " + state_id);
        // present lobby ui that visualizes all connected players

        coroutine = StartCoroutine(connectionsCheck(connection_check_frequency));
    }

    public void execute(){

        // check if a specific event occured which indicates a following state exit
        if(Input.GetKeyDown("k")){ // testwise press k
            finished = true;
        }
    }

    public void exit(){
        Debug.Log("Exit state : " + state_id);
        StopCoroutine(coroutine);
        // remove lobby ui

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


    private IEnumerator connectionsCheck(float sec){
        yield return new WaitForSeconds(sec);
        server.connectionsCheck();
        coroutine = StartCoroutine(connectionsCheck(connection_check_frequency));
    }

    public void handlePlayerKey(int id,bool key_state, PlayerKeys player_key){

    }

    public void handlePlayerDirection(int id,float mouse_angle){
        
    }

    public void finishState(){
        finished = true;
    }

}
