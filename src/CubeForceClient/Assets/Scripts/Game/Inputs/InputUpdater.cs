using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 
using System.Text; 

public class InputUpdater : MonoBehaviour
{
    // Update is called once per frame

    

    private Client client;
    private Game game;

    void Start(){
        client = GameObject.Find("Client").GetComponent<Client>();
        game = GameObject.Find("Game").GetComponent<Game>();
    }

    void Update()
    {
        if(Client.id == -1) return;
        
        if(Input.GetMouseButtonDown(0)){
            Packet packet = PacketManufacturer.playerKeyShoot(Client.id);
            client.sendMessageReliable(packet);
        }
        if(Input.GetKeyDown("space")){
            game.players[Client.id].transform.GetChild(0).GetComponent<PlayerAttributes>().key_dodge = true;
            Packet packet = PacketManufacturer.playerKeyDodge(Client.id);
            client.sendMessageReliable(packet);
        }
        if(Input.GetKeyDown(KeyCode.LeftShift)){
            Packet packet = PacketManufacturer.playerKeyShield(Client.id);
            client.sendMessageReliable(packet);
        }
        if(Input.GetKeyDown("w")){
            game.players[Client.id].transform.GetChild(0).GetComponent<PlayerAttributes>().key_move_up = true;
            Packet packet = PacketManufacturer.playerKeyMoveUp(Client.id,true);
            client.sendMessageReliable(packet);
        }
        if(Input.GetKeyDown("a")){
            game.players[Client.id].transform.GetChild(0).GetComponent<PlayerAttributes>().key_move_left = true;
            Packet packet = PacketManufacturer.playerKeyMoveLeft(Client.id,true);
            client.sendMessageReliable(packet);
        }
        if(Input.GetKeyDown("s")){
            game.players[Client.id].transform.GetChild(0).GetComponent<PlayerAttributes>().key_move_down = true;
            Packet packet = PacketManufacturer.playerKeyMoveDown(Client.id,true);
            client.sendMessageReliable(packet);
        }
        if(Input.GetKeyDown("d")){
            game.players[Client.id].transform.GetChild(0).GetComponent<PlayerAttributes>().key_move_right = true;
            Packet packet = PacketManufacturer.playerKeyMoveRight(Client.id,true);
            client.sendMessageReliable(packet);
        }


        if(Input.GetKeyUp("w")){
            game.players[Client.id].transform.GetChild(0).GetComponent<PlayerAttributes>().key_move_up = false;
            Packet packet = PacketManufacturer.playerKeyMoveUp(Client.id,false);
            client.sendMessageReliable(packet);
        }
        if(Input.GetKeyUp("a")){
            game.players[Client.id].transform.GetChild(0).GetComponent<PlayerAttributes>().key_move_left = false;
            Packet packet = PacketManufacturer.playerKeyMoveLeft(Client.id,false);
            client.sendMessageReliable(packet);
        }
        if(Input.GetKeyUp("s")){
            game.players[Client.id].transform.GetChild(0).GetComponent<PlayerAttributes>().key_move_down = false;
            Packet packet = PacketManufacturer.playerKeyMoveDown(Client.id,false);
            client.sendMessageReliable(packet);
        }
        if(Input.GetKeyUp("d")){
            game.players[Client.id].transform.GetChild(0).GetComponent<PlayerAttributes>().key_move_right = false;
            Packet packet = PacketManufacturer.playerKeyMoveRight(Client.id,false);
            client.sendMessageReliable(packet);
        }
    }

}
