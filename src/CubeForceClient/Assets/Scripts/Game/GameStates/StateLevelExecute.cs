using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StateLevelExecute : MonoBehaviour, IState
{
    public StateID state_id;


    private Coroutine coroutine;


    private Game game;

    public ScoreBoard score_board;

    public void Awake(){
        game = GameObject.Find("Game").GetComponent<Game>();
    }

    public void enter(){
        Debug.Log("Enter state : " + state_id);
    }

    public void execute(){
        score_board.update();
    }

    public void exit(){
        Debug.Log("Exit state : " + state_id);
    }


    public StateID getStateID(){
        return state_id;
    }




}
