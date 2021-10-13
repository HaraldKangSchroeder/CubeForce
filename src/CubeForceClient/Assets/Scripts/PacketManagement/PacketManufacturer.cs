using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;



public class PacketManufacturer
{

    public static Packet playerReady(int id){
        PlayerSequenceNumbersClient sequence_numbers = GameObject.Find("Game").GetComponent<Game>().players[id].transform.GetChild(0).GetComponent<PlayerSequenceNumbersClient>();

        Packet packet = new Packet(PacketTypeClient.PLAYER_READY);
        packet.writeByte((byte) id);
        packet.writeInt(++sequence_numbers.ready);
        return packet;
    }
    
    public static Packet playerKeys(int id, bool[] keys){
        PlayerAttributes player_attributes = GameObject.Find("Game").GetComponent<Game>().players[id].transform.GetChild(0).GetComponent<PlayerAttributes>();
        PlayerSequenceNumbersClient sequence_numbers = GameObject.Find("Game").GetComponent<Game>().players[id].transform.GetChild(0).GetComponent<PlayerSequenceNumbersClient>();

        Packet packet = new Packet(PacketTypeClient.PLAYER_KEYS);
        packet.writeByte((byte) id);
        packet.writeInt(++sequence_numbers.keys);
        for(int i = 0; i < keys.Length; i++){
            packet.writeBool(keys[i]);
        }
        return packet;
    }

    public static Packet playerKeyMoveUp(int id,bool b){
        PlayerSequenceNumbersClient sequence_numbers = GameObject.Find("Game").GetComponent<Game>().players[id].transform.GetChild(0).GetComponent<PlayerSequenceNumbersClient>();

        Packet packet = new Packet(PacketTypeClient.PLAYER_KEY_MOVE_UP);
        packet.writeByte((byte)id);
        packet.writeInt(++sequence_numbers.key_move_up);
        packet.writeBool(b);
        return packet;
    }

    public static Packet playerKeyMoveLeft(int id,bool b){
        PlayerSequenceNumbersClient sequence_numbers = GameObject.Find("Game").GetComponent<Game>().players[id].transform.GetChild(0).GetComponent<PlayerSequenceNumbersClient>();
        
        Packet packet = new Packet(PacketTypeClient.PLAYER_KEY_MOVE_LEFT);
        packet.writeByte((byte)id);
        packet.writeInt(++sequence_numbers.key_move_left);
        packet.writeBool(b);
        return packet;
    }

    public static Packet playerKeyMoveDown(int id,bool b){
        PlayerSequenceNumbersClient sequence_numbers = GameObject.Find("Game").GetComponent<Game>().players[id].transform.GetChild(0).GetComponent<PlayerSequenceNumbersClient>();
        
        Packet packet = new Packet(PacketTypeClient.PLAYER_KEY_MOVE_DOWN);
        packet.writeByte((byte)id);
        packet.writeInt(++sequence_numbers.key_move_down);
        packet.writeBool(b);
        return packet;
    }

    public static Packet playerKeyMoveRight(int id,bool b){
        PlayerSequenceNumbersClient sequence_numbers = GameObject.Find("Game").GetComponent<Game>().players[id].transform.GetChild(0).GetComponent<PlayerSequenceNumbersClient>();
        
        Packet packet = new Packet(PacketTypeClient.PLAYER_KEY_MOVE_RIGHT);
        packet.writeByte((byte)id);
        packet.writeInt(++sequence_numbers.key_move_right);
        packet.writeBool(b);
        return packet;
    }

    public static Packet playerKeyShoot(int id){
        PlayerSequenceNumbersClient sequence_numbers = GameObject.Find("Game").GetComponent<Game>().players[id].transform.GetChild(0).GetComponent<PlayerSequenceNumbersClient>();
        
        Packet packet = new Packet(PacketTypeClient.PLAYER_KEY_SHOOT);
        packet.writeByte((byte)id);
        packet.writeInt(++sequence_numbers.key_shoot);
        return packet;
    }

    public static Packet playerKeyDodge(int id){
        PlayerSequenceNumbersClient sequence_numbers = GameObject.Find("Game").GetComponent<Game>().players[id].transform.GetChild(0).GetComponent<PlayerSequenceNumbersClient>();
        
        Packet packet = new Packet(PacketTypeClient.PLAYER_KEY_DODGE);
        packet.writeByte((byte)id);
        packet.writeInt(++sequence_numbers.key_dodge);
        return packet;
    }

    public static Packet playerKeyShield(int id){
        PlayerSequenceNumbersClient sequence_numbers = GameObject.Find("Game").GetComponent<Game>().players[id].transform.GetChild(0).GetComponent<PlayerSequenceNumbersClient>();
        
        Packet packet = new Packet(PacketTypeClient.PLAYER_KEY_SHIELD);
        packet.writeByte((byte)id);
        packet.writeInt(++sequence_numbers.key_shield);
        return packet;
    }

    public static Packet playerMouseRotationAngle(int id){
        PlayerAttributes player_attributes = GameObject.Find("Game").GetComponent<Game>().players[id].transform.GetChild(0).GetComponent<PlayerAttributes>();
        PlayerSequenceNumbersClient sequence_numbers = GameObject.Find("Game").GetComponent<Game>().players[id].transform.GetChild(0).GetComponent<PlayerSequenceNumbersClient>();

        Packet packet = new Packet(PacketTypeClient.MOUSE_ROTATION_ANGLE);
        packet.writeByte((byte)id);
        packet.writeInt(++sequence_numbers.mouse_angle);
        packet.writeFloat(player_attributes.angle_to_mouse);
        return packet;
    }


    public static Packet clientIntroduction(string name){
        Packet packet = new Packet(PacketTypeClient.CLIENT_INTRODUCTION);
        packet.writeString(name);
        return packet;
    }

    public static Packet clientAdmissionConfirmation(int id){
        Packet packet = new Packet(PacketTypeClient.CLIENT_ADMISSION_CONFIRMATION);
        packet.writeByte((byte)id);
        return packet;
    }

    public static Packet messageAcknowledgment(int id, int msg_id){
        Packet packet = new Packet(PacketTypeClient.MESSAGE_ACKNOWLEDGMENT);
        packet.writeByte((byte)id);
        packet.writeByte((byte)msg_id);
        return packet;
    }

    public static Packet startGame(int id){
        Packet packet = new Packet(PacketTypeClient.START_GAME);
        packet.writeByte((byte)id);
        return packet;
    }

    public static Packet toLobby(int id){
        Packet packet = new Packet(PacketTypeClient.TO_LOBBY);
        packet.writeByte((byte)id);
        return packet;
    }
}
