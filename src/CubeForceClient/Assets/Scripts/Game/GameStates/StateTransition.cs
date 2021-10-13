using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateTransition : MonoBehaviour, IState
{
    public StateID state_id;

    // internal state variables
    private Game game;

    public void Awake(){
        game = GameObject.Find("Game").GetComponent<Game>();
    }

    public void enter(){
        Debug.Log("Enter state : " + state_id);
        // present lobby ui that visualizes all connected players

        // remove remaining components before entering the next state
        game.destroyAllCorpses();
        // remove arena object of lvl 0
        // move occluders and player that is alive (if existent) updwards. delete occluders afterwards;
        game.occluders.GetComponent<Animator>().Play("OccludersFadeOut");
        

    }

    public void execute(){

    }

    public void exit(){
        Debug.Log("Exit state : " + state_id);
        // remove lobby ui
        Destroy(game.occluders);
    }


    public StateID getStateID(){
        return state_id;
    }



}
