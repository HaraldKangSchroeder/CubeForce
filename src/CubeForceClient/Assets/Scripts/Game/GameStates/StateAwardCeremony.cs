using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateAwardCeremony : MonoBehaviour, IState
{
    public StateID state_id;

    public AwardCeremonyBoard award_ceremony_board;

    public void enter(){
        Debug.Log("Enter state : " + state_id);
        award_ceremony_board.openBoard();
        // present award ceremony ui that visualizes the ranking of this round
    }

    public void execute(){
        

        
    }

    public void exit(){
        Debug.Log("Exit state : " + state_id);
        award_ceremony_board.closeBoard();
        // remove award ceremony ui
    }


    public StateID getStateID(){
        return state_id;
    }



    
}
