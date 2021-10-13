using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateLobby : MonoBehaviour, IState
{
    public StateID state_id;

    public LobbyBoard lobby_board;
    
    public GameObject munition_ui;
    public GameObject shield_ui;

    public void enter(){
        Debug.Log("Enter state : " + state_id);
        lobby_board.openBoard();
        // present lobby ui that visualizes all connected players
    }

    public void execute(){
        lobby_board.update();
    }

    public void exit(){
        Debug.Log("Exit state : " + state_id);
        lobby_board.closeBoard();
        
        // show ui game specific elements
        munition_ui.active = true;
        shield_ui.active = true;
    }


    public StateID getStateID(){
        return state_id;
    }




}
