using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyBoard : MonoBehaviour
{

    public GameObject player_data_field;
    public GameObject player_data_prefab;

    public GameObject start_game_button;
    public Image button_ready;

    public bool should_update = false;

    private int start_top = -13;
    private int step_size = 6;


    public void update(){
        if(should_update){
            resetContent();
            should_update = false;
        }
    }

    public void openBoard(){
        gameObject.SetActive(true);
        setContent();
    }

    public void closeBoard(){
        gameObject.SetActive(false);
        removeContent();
    }

    private void resetContent(){
        removeContent();
        setContent();
    }

    private void removeContent(){
        foreach (Transform child in player_data_field.transform) {
            GameObject.Destroy(child.gameObject);
        }
    }

    private void setContent(){
        Game game = GameObject.Find("Game").GetComponent<Game>(); 
        int players_count = 0;
        bool all_ready = true;

        for(int i = 0; i < game.players.Length; i++){
            if(game.players[i] != null){
                players_count++;
                GameObject player_data_object = Instantiate(player_data_prefab);
                PlayerAttributes player_attributes = game.players[i].transform.GetChild(0).GetComponent<PlayerAttributes>();
                player_data_object.GetComponent<RectTransform>().localPosition  = new Vector3(0, start_top - i * step_size,0);
                player_data_object.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = player_attributes.name;
                player_data_object.transform.GetChild(1).gameObject.SetActive(player_attributes.ready);
                player_data_object.transform.GetChild(2).gameObject.SetActive(player_attributes.id == Client.host_id);
                
                player_data_object.transform.SetParent(player_data_field.transform,false);
                if(!player_attributes.ready){
                    all_ready = false;
                }
            }
        }
        if(Client.id == Client.host_id){
            start_game_button.SetActive(Client.id == Client.host_id);
            //if(all_ready && players_count > 1){
            if(all_ready){
                start_game_button.GetComponent<Image>().color = Color.white;
                start_game_button.GetComponent<Button>().interactable = true;
            }
            else{
                start_game_button.GetComponent<Image>().color = new Color(0.5f,0.5f,0.5f);
                start_game_button.GetComponent<Button>().interactable = false;
            }
        }
        else{
            start_game_button.SetActive(false);
        }

        if(Client.id != -1 && game.players[Client.id] != null){
            button_ready.color = game.players[Client.id].transform.GetChild(0).GetComponent<PlayerAttributes>().ready ? Color.green : Color.red;
        }
        
    }



    public void changeReady(){
        Packet packet = PacketManufacturer.playerReady(Client.id);
        GameObject.Find("Client").GetComponent<Client>().sendMessageReliable(packet);
    }

    public void startGame(){
        Packet packet = PacketManufacturer.startGame(Client.id);
        GameObject.Find("Client").GetComponent<Client>().sendMessageReliable(packet);
    }

}
