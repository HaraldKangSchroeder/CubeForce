using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;



public class PacketProcessor
{
    public static bool checkValidity(Packet packet){
        if(packet.getAbsolutePacketLength() <= 1){
            return false;
        }
        int length = (int)packet.readByte();
        return length == packet.getAbsolutePacketLength() - 1;
    }


    #region  processing methods - they all assume that one readByte (for length) and one readByte (for message type) were done before

    public static void messageAcknowledgment(Packet packet){
        int id = (int)packet.readByte();
        int message_id = (int)packet.readByte();
        Server server = GameObject.Find("Server").GetComponent<Server>();
        if(server.clients[id] != null){
            if(server.clients[id].isRunningMessage(message_id)){
                server.clients[id].removeRunningMessage(message_id);
            }
        }
    }
    
    // public static void playerKeys(Packet packet){
    //     int id = (int)packet.readByte();
    //     int sequence_number = (int)packet.readInt();

    //     Game game = GameObject.Find("Game").GetComponent<Game>();
    //     if(game.players[id] != null){
    //         PlayerSequenceNumbersClient sequence_numbers = game.players[id].GetComponent<PlayerSequenceNumbersClient>();
    //         if(sequence_number > sequence_numbers.keys){
    //             sequence_numbers.keys = sequence_number;
    //             bool[] keys = new bool[] {packet.readBool(),packet.readBool(),packet.readBool(),packet.readBool(),packet.readBool(),packet.readBool(),packet.readBool()};
    //             game.handlePlayerKeys(id,keys);
    //         }
    //     }
    // }

    public static void playerMouseRotationAngle(Packet packet){
        int id = (int)packet.readByte();
        int sequence_number = (int)packet.readInt();

        Game game = GameObject.Find("Game").GetComponent<Game>();
        if(game.players[id] != null){
            PlayerSequenceNumbersClient sequence_numbers = game.players[id].GetComponent<PlayerSequenceNumbersClient>();
            if(sequence_number > sequence_numbers.mouse_angle){
                sequence_numbers.mouse_angle = sequence_number;
                game.handlePlayerDirection(id,packet.readFloat());
            }
        }
    }


    public static void playerReady(Packet packet){
        int id = (int)packet.readByte();
        int sequence_number = packet.readInt();

        Server server = GameObject.Find("Server").GetComponent<Server>();
        Game game = GameObject.Find("Game").GetComponent<Game>();
        if(game.players[id] != null){
            PlayerSequenceNumbersClient sequence_numbers = game.players[id].GetComponent<PlayerSequenceNumbersClient>();
            if(sequence_number > sequence_numbers.ready){
                sequence_numbers.ready = sequence_number;
                game.players[id].GetComponent<PlayerAttributes>().ready = !game.players[id].GetComponent<PlayerAttributes>().ready; 
                Packet packet_response = PacketManufacturer.playerReady(id);
                server.sendMessageBroadcastReliable(packet_response);
            }
        }
    }


    public static void clientIntroduction(Packet packet, IPEndPoint ep){
        Server server = GameObject.Find("Server").GetComponent<Server>();

        //prevent multiple subsequent itroductions of the same player to be executed multiple times as well
        if(!server.isClient(ep) && !server.isGameFullyOccupied() && server.isGameInLobby()){
            string name = packet.readString();
            int id = server.storeClient(ep,name);

            Packet packet_confirmation = PacketManufacturer.clientAdmissionConfirmation(id);
            server.sendMessageReliable(id,packet_confirmation);

        }
    }

    public static void clientAdmissionConfirmation(Packet packet){
        Server server = GameObject.Find("Server").GetComponent<Server>();

        int id = (int)packet.readByte();
        // spawn player with same id in the game
        GameObject.Find("Game").GetComponent<Game>().createPlayer(id,server.clients[id].name);

        Packet packet_introduction = PacketManufacturer.playerIntroduction(id);
        server.sendMessageBroadcastExceptOneReliable(id,packet_introduction);

        for(int i = 0; i < server.clients.Length; i++){
            if(server.clients[i] != null){
                Packet packet0 = PacketManufacturer.playerIntroduction(server.clients[i].id);
                server.sendMessageReliable(id,packet0);

                packet0 = PacketManufacturer.playerReady(server.clients[i].id);
                server.sendMessageReliable(id,packet0);
            }
        }

        // tell the new connected player who the current host is
        Packet packet_host = PacketManufacturer.clientHost();
        server.sendMessageReliable(id,packet_host);

    }

    public static void startGame(Packet packet){
        Server server = GameObject.Find("Server").GetComponent<Server>();
        int id = (int)packet.readByte();
        if(id == server.host_id){
            Game game = GameObject.Find("Game").GetComponent<Game>();
            if(game.getGameStateID() == StateID.LOBBY){
                game.current_state.finishState();
                // unready all players so that they are not ready when they get back to the lobby
                foreach(GameObject player in game.players)
                {
                    if(player != null)
                    {
                        player.GetComponent<PlayerAttributes>().ready = false;
                    }
                }
                packet = PacketManufacturer.playersUnreadyAll();
                server.sendMessageBroadcastReliable(packet);
            }
        }
    }

    public static void toLobby(Packet packet){
        Server server = GameObject.Find("Server").GetComponent<Server>();
        int id = (int)packet.readByte();
        if(id == server.host_id){
            Game game = GameObject.Find("Game").GetComponent<Game>();
            if(game.getGameStateID() == StateID.AWARD_CEREMONY){
                game.current_state.finishState();
            }
        }
    }


    public static void playerKeyMoveUp(Packet packet){
        int id = (int)packet.readByte();
        int sequence_number = packet.readInt();

        Game game = GameObject.Find("Game").GetComponent<Game>();
        if(game.players[id] != null){
            PlayerSequenceNumbersClient sequence_numbers = game.players[id].GetComponent<PlayerSequenceNumbersClient>();
            if(sequence_number > sequence_numbers.key_move_up){
                sequence_numbers.key_move_up = sequence_number;
                //game.players[id].GetComponent<PlayerAttributes>().key_move_up = packet.readBool(); 
                game.handlePlayerKey(id,packet.readBool(),PlayerKeys.MOVE_UP);

                Packet packet_response = PacketManufacturer.playerKeyMoveUp(id);
                GameObject.Find("Server").GetComponent<Server>().sendMessageBroadcastExceptOneReliable(id,packet_response);
            }
        }
    }

    public static void playerKeyMoveLeft(Packet packet){
        int id = (int)packet.readByte();
        int sequence_number = packet.readInt();

        Game game = GameObject.Find("Game").GetComponent<Game>();
        if(game.players[id] != null){
            PlayerSequenceNumbersClient sequence_numbers = game.players[id].GetComponent<PlayerSequenceNumbersClient>();
            if(sequence_number > sequence_numbers.key_move_left){
                sequence_numbers.key_move_left = sequence_number;
                //game.players[id].GetComponent<PlayerAttributes>().key_move_left = packet.readBool(); 
                game.handlePlayerKey(id,packet.readBool(),PlayerKeys.MOVE_LEFT);

                Packet packet_response = PacketManufacturer.playerKeyMoveLeft(id);
                GameObject.Find("Server").GetComponent<Server>().sendMessageBroadcastExceptOneReliable(id,packet_response);
            }
        }
    }

    public static void playerKeyMoveDown(Packet packet){
        int id = (int)packet.readByte();
        int sequence_number = packet.readInt();

        Game game = GameObject.Find("Game").GetComponent<Game>();
        if(game.players[id] != null){
            PlayerSequenceNumbersClient sequence_numbers = game.players[id].GetComponent<PlayerSequenceNumbersClient>();
            if(sequence_number > sequence_numbers.key_move_down){
                sequence_numbers.key_move_down = sequence_number;
                //game.players[id].GetComponent<PlayerAttributes>().key_move_down = packet.readBool(); 
                game.handlePlayerKey(id,packet.readBool(),PlayerKeys.MOVE_DOWN);

                Packet packet_response = PacketManufacturer.playerKeyMoveDown(id);
                GameObject.Find("Server").GetComponent<Server>().sendMessageBroadcastExceptOneReliable(id,packet_response);
            }
        }
    }

    public static void playerKeyMoveRight(Packet packet){
        int id = (int)packet.readByte();
        int sequence_number = packet.readInt();

        Game game = GameObject.Find("Game").GetComponent<Game>();
        if(game.players[id] != null){
            PlayerSequenceNumbersClient sequence_numbers = game.players[id].GetComponent<PlayerSequenceNumbersClient>();
            if(sequence_number > sequence_numbers.key_move_right){
                sequence_numbers.key_move_right = sequence_number;
                //game.players[id].GetComponent<PlayerAttributes>().key_move_right = packet.readBool(); 
                game.handlePlayerKey(id,packet.readBool(),PlayerKeys.MOVE_RIGHT);

                Packet packet_response = PacketManufacturer.playerKeyMoveRight(id);
                GameObject.Find("Server").GetComponent<Server>().sendMessageBroadcastExceptOneReliable(id,packet_response);
            }
        }
    }

    public static void playerKeyShield(Packet packet){
        int id = (int)packet.readByte();
        int sequence_number = packet.readInt();

        Game game = GameObject.Find("Game").GetComponent<Game>();
        if(game.players[id] != null){
            PlayerSequenceNumbersClient sequence_numbers = game.players[id].GetComponent<PlayerSequenceNumbersClient>();
            if(sequence_number > sequence_numbers.key_shield){
                sequence_numbers.key_shield = sequence_number;
                //game.players[id].GetComponent<PlayerAttributes>().key_shield = true; 
                game.handlePlayerKey(id,true,PlayerKeys.SHIELD);
            }
        }
    }

    public static void playerKeyShoot(Packet packet){
        int id = (int)packet.readByte();
        int sequence_number = packet.readInt();

        Game game = GameObject.Find("Game").GetComponent<Game>();
        if(game.players[id] != null){
            PlayerSequenceNumbersClient sequence_numbers = game.players[id].GetComponent<PlayerSequenceNumbersClient>();
            if(sequence_number > sequence_numbers.key_shoot){
                sequence_numbers.key_shoot = sequence_number;
                //game.players[id].GetComponent<PlayerAttributes>().key_shoot = true; 
                game.handlePlayerKey(id,true,PlayerKeys.SHOOT);
            }
        }
    }

    public static void playerKeyDodge(Packet packet){
        int id = (int)packet.readByte();
        int sequence_number = packet.readInt();

        Game game = GameObject.Find("Game").GetComponent<Game>();
        if(game.players[id] != null){
            PlayerSequenceNumbersClient sequence_numbers = game.players[id].GetComponent<PlayerSequenceNumbersClient>();
            if(sequence_number > sequence_numbers.key_dodge){
                sequence_numbers.key_dodge = sequence_number;
                //game.players[id].GetComponent<PlayerAttributes>().key_dodge = true; 
                game.handlePlayerKey(id,true,PlayerKeys.DODGE);

                Packet packet_response = PacketManufacturer.playerKeyDodge(id);
                GameObject.Find("Server").GetComponent<Server>().sendMessageBroadcastExceptOneReliable(id,packet_response);
            }
        }
    }


    #endregion


    public static void processPacket(Packet packet, PacketTypeClient packet_type_client, IPEndPoint ep){
        ThreadManager.ExecuteOnMainThread(() => {
            Game game = GameObject.Find("Game").GetComponent<Game>();
            
            switch(packet_type_client)
            {
                // case PacketTypeClient.PLAYER_KEYS:
                //     playerKeys(packet);
                //     break;
                case PacketTypeClient.MOUSE_ROTATION_ANGLE:
                    playerMouseRotationAngle(packet);
                    break;
                case PacketTypeClient.MESSAGE_ACKNOWLEDGMENT:
                    messageAcknowledgment(packet);
                    break;
                case PacketTypeClient.CLIENT_INTRODUCTION:
                    clientIntroduction(packet,ep);
                    break;
                case PacketTypeClient.PLAYER_READY:
                    playerReady(packet);
                    break;
                case PacketTypeClient.CLIENT_ADMISSION_CONFIRMATION:
                    clientAdmissionConfirmation(packet);
                    break;
                case PacketTypeClient.START_GAME:
                    startGame(packet);
                    break;
                case PacketTypeClient.PLAYER_KEY_MOVE_UP:
                    playerKeyMoveUp(packet);
                    break;
                case PacketTypeClient.PLAYER_KEY_MOVE_LEFT:
                    playerKeyMoveLeft(packet);
                    break;
                case PacketTypeClient.PLAYER_KEY_MOVE_DOWN:
                    playerKeyMoveDown(packet);
                    break;
                case PacketTypeClient.PLAYER_KEY_MOVE_RIGHT:
                    playerKeyMoveRight(packet);
                    break;
                case PacketTypeClient.PLAYER_KEY_SHOOT:
                    playerKeyShoot(packet);
                    break;
                case PacketTypeClient.PLAYER_KEY_SHIELD:
                    playerKeyShield(packet);
                    break;
                case PacketTypeClient.PLAYER_KEY_DODGE:
                    playerKeyDodge(packet);
                    break;
                case PacketTypeClient.TO_LOBBY:
                    toLobby(packet);
                    break;
                default:
                    break;
            }
        });
    }
}
