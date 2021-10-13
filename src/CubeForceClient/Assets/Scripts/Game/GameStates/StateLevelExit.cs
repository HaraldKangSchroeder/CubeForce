using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateLevelExit : MonoBehaviour, IState
{
    public StateID state_id;

    private bool coroutine_running;


    private Game game;

    public void Awake(){
        game = GameObject.Find("Game").GetComponent<Game>();
    }

    public void enter(){
        Debug.Log("Enter state : " + state_id);
        //game.arena.GetComponent<Animator>().SetBool("destroyed",true);
    }

    public void execute(){

    }

    public void exit(){
        Debug.Log("Exit state : " + state_id);

    }


    public StateID getStateID(){
        return state_id;
    }



    private IEnumerator FinishWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        coroutine_running = false;
    }

}
