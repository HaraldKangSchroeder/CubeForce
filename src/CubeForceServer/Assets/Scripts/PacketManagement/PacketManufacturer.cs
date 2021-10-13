using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;




public static class PacketManufacturer
{

    public static Packet connectionCheck()
    {
        Packet packet = new Packet(PacketTypeServer.CONNECTION_CHECK);
        return packet;
    }

    public static Packet clientAdmissionConfirmation(int id)
    {
        Packet packet = new Packet(PacketTypeServer.CLIENT_ADMISSION_CONFIRMATION);
        packet.writeByte((byte)id);
        return packet;
    }

    public static Packet playerIntroduction(int id)
    {
        PlayerAttributes player_attributes = GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerAttributes>();

        Packet packet = new Packet(PacketTypeServer.PLAYER_INTRODUCTION);
        packet.writeByte((byte)id);
        packet.writeVector3(player_attributes.transform.position);
        packet.writeString(player_attributes.player_name);
        return packet;
    }

    public static Packet messageAcknowledgment(int msg_id)
    {
        Packet packet = new Packet(PacketTypeServer.MESSAGE_ACKNOWLEDGMENT);
        packet.writeByte((byte)msg_id);
        return packet;
    }


    public static Packet clientDisconnect(int id)
    {
        Packet packet = new Packet(PacketTypeServer.CLIENT_DISCONNECT);
        packet.writeByte((byte)id);
        return packet;
    }

    public static Packet playerTransform(int id)
    {
        Game game = GameObject.Find("Game").GetComponent<Game>();
        PlayerAttributes player_attributes = game.players[id].GetComponent<PlayerAttributes>();
        PlayerSequenceNumbersServer sequence_numbers = game.players[id].GetComponent<PlayerSequenceNumbersServer>();
        Packet packet = new Packet(PacketTypeServer.PLAYER_TRANSFORM);
        packet.writeByte((byte) id);
        packet.writeInt(++sequence_numbers.player_transform);
        packet.writeVector3(player_attributes.transform.position);
        packet.writeVector3(player_attributes.transform.rotation.eulerAngles);
        return packet;
    }

    public static Packet playersTransform()
    {
        Packet packet = new Packet(PacketTypeServer.PLAYERS_TRANSFORM);
        Game game = GameObject.Find("Game").GetComponent<Game>();
        GameSequenceNumbers sequence_numbers = game.GetComponent<GameSequenceNumbers>();
        packet.writeInt(++sequence_numbers.players_transform);
        foreach (GameObject player in game.players)
        {
            if (player != null)
            {
                PlayerAttributes player_attributes = player.GetComponent<PlayerAttributes>();
                packet.writeByte((byte)player_attributes.id);
                packet.writeVector3(player_attributes.transform.position);
                packet.writeVector3(player_attributes.transform.rotation.eulerAngles);
            }
        }
        return packet;
    }

    public static Packet projectilesTransform()
    {
        Packet packet = new Packet(PacketTypeServer.PROJECTILES_TRANSFORM);
        Game game = GameObject.Find("Game").GetComponent<Game>();
        GameSequenceNumbers sequence_numbers = game.GetComponent<GameSequenceNumbers>();
        packet.writeInt(++sequence_numbers.projectiles_transform);
        foreach(GameObject projectile in game.projectiles.Values)
        {
            ProjectileAttributes projectile_attributes = projectile.GetComponent<ProjectileAttributes>();
            packet.writeByte((byte)projectile_attributes.id);
            packet.writeVector3(projectile_attributes.transform.position);
        }
        return packet;
    }

    public static Packet boostpacketsTransform()
    {
        Packet packet = new Packet(PacketTypeServer.BOOSTPACKETS_TRANSFORM);
        Game game = GameObject.Find("Game").GetComponent<Game>();
        GameSequenceNumbers sequence_numbers = game.GetComponent<GameSequenceNumbers>();
        packet.writeInt(++sequence_numbers.boostpackets_transform);
        foreach(GameObject boostpacket in game.boostpackets.Values)
        {
            BoostpacketAttributes boostpacket_attributes = boostpacket.GetComponent<BoostpacketAttributes>();
            packet.writeByte((byte)boostpacket_attributes.id);
            packet.writeVector3(boostpacket_attributes.transform.position);
            packet.writeVector3(boostpacket_attributes.transform.rotation.eulerAngles);
        }
        return packet;
    }

    public static Packet playerPosition(int id)
    {
        PlayerAttributes player_attributes = GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerAttributes>();
        PlayerSequenceNumbersServer sequence_numbers = GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerSequenceNumbersServer>();
        Packet packet = new Packet(PacketTypeServer.PLAYER_POSITION);
        packet.writeByte((byte)id);
        packet.writeInt(++sequence_numbers.position);
        packet.writeVector3(player_attributes.transform.position);
        return packet;
    }

    public static Packet playerRotation(int id)
    {
        PlayerAttributes player_attributes = GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerAttributes>();
        PlayerSequenceNumbersServer sequence_numbers = GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerSequenceNumbersServer>();

        Packet packet = new Packet(PacketTypeServer.PLAYER_ROTATION);
        packet.writeByte((byte)id);
        packet.writeInt(++sequence_numbers.rotation);
        packet.writeVector3(player_attributes.transform.rotation.eulerAngles);
        return packet;
    }

    public static Packet playerAlive(int id)
    {
        PlayerAttributes player_attributes = GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerAttributes>();
        PlayerSequenceNumbersServer sequence_numbers = GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerSequenceNumbersServer>();

        Packet packet = new Packet(PacketTypeServer.PLAYER_ALIVE);
        packet.writeByte((byte)id);
        packet.writeInt(++sequence_numbers.alive);
        packet.writeBool(player_attributes.alive);
        return packet;
    }

    public static Packet playerOnFire(int id)
    {
        PlayerAttributes player_attributes = GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerAttributes>();
        PlayerSequenceNumbersServer sequence_numbers = GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerSequenceNumbersServer>();

        Packet packet = new Packet(PacketTypeServer.PLAYER_ON_FIRE);
        packet.writeByte((byte)id);
        packet.writeInt(++sequence_numbers.on_fire);
        packet.writeBool(player_attributes.on_fire);
        return packet;
    }

    public static Packet playerOnSpeed(int id)
    {
        PlayerAttributes player_attributes = GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerAttributes>();
        PlayerSequenceNumbersServer sequence_numbers = GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerSequenceNumbersServer>();

        Packet packet = new Packet(PacketTypeServer.PLAYER_ON_SPEED);
        packet.writeByte((byte)id);
        packet.writeInt(++sequence_numbers.on_speed);
        packet.writeBool(player_attributes.on_speed);
        return packet;
    }

    public static Packet playerMunition(int id)
    {
        PlayerAttributes player_attributes = GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerAttributes>();
        PlayerSequenceNumbersServer sequence_numbers = GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerSequenceNumbersServer>();

        Packet packet = new Packet(PacketTypeServer.PLAYER_MUNITION);
        packet.writeByte((byte)id);
        packet.writeInt(++sequence_numbers.munition);
        packet.writeByte((byte)player_attributes.munition);
        return packet;
    }

    public static Packet playerSideStepReady(int id)
    {
        PlayerAttributes player_attributes = GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerAttributes>();
        PlayerSequenceNumbersServer sequence_numbers = GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerSequenceNumbersServer>();
        Packet packet = new Packet(PacketTypeServer.PLAYER_SIDESTEP_READY);
        packet.writeByte((byte)id);
        packet.writeInt(++sequence_numbers.sidestep_ready);
        packet.writeBool(player_attributes.sidestep_ready);
        return packet;
    }

    public static Packet playerShieldReady(int id)
    {
        PlayerAttributes player_attributes = GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerAttributes>();
        PlayerSequenceNumbersServer sequence_numbers = GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerSequenceNumbersServer>();

        Packet packet = new Packet(PacketTypeServer.PLAYER_SHIELD_READY);
        packet.writeByte((byte)id);
        packet.writeInt(++sequence_numbers.shield_ready);
        packet.writeBool(player_attributes.shield_ready);
        return packet;
    }

    public static Packet playerShieldActive(int id)
    {
        PlayerAttributes player_attributes = GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerAttributes>();
        PlayerSequenceNumbersServer sequence_numbers = GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerSequenceNumbersServer>();

        Packet packet = new Packet(PacketTypeServer.PLAYER_SHIELD_ACTIVE);
        packet.writeByte((byte)id);
        packet.writeInt(++sequence_numbers.shield_active);
        packet.writeBool(player_attributes.shield_active);
        return packet;
    }

    public static Packet playerKills(int id)
    {
        PlayerAttributes player_attributes = GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerAttributes>();
        PlayerSequenceNumbersServer sequence_numbers = GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerSequenceNumbersServer>();

        Packet packet = new Packet(PacketTypeServer.PLAYER_KILLS);
        packet.writeByte((byte)id);
        packet.writeInt(++sequence_numbers.kills);
        packet.writeInt(player_attributes.kills);
        return packet;
    }

    public static Packet playerWins(int id)
    {
        PlayerAttributes player_attributes = GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerAttributes>();
        PlayerSequenceNumbersServer sequence_numbers = GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerSequenceNumbersServer>();

        Packet packet = new Packet(PacketTypeServer.PLAYER_WINS);
        packet.writeByte((byte)id);
        packet.writeInt(++sequence_numbers.wins);
        packet.writeInt(player_attributes.wins);
        return packet;
    }

    public static Packet playerKeys(int id)
    { //wasd
        PlayerAttributes player_attributes = GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerAttributes>();
        PlayerSequenceNumbersServer sequence_numbers = GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerSequenceNumbersServer>();

        Packet packet = new Packet(PacketTypeServer.PLAYER_KEYS);
        packet.writeByte((byte)id);
        packet.writeInt(++sequence_numbers.keys);
        packet.writeBool(player_attributes.key_move_up);
        packet.writeBool(player_attributes.key_move_left);
        packet.writeBool(player_attributes.key_move_down);
        packet.writeBool(player_attributes.key_move_right);
        return packet;
    }

    public static Packet playerKeyMoveUp(int id)
    {
        PlayerAttributes player_attributes = GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerAttributes>();
        PlayerSequenceNumbersServer sequence_numbers = GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerSequenceNumbersServer>();

        Packet packet = new Packet(PacketTypeServer.PLAYER_KEY_MOVE_UP);
        packet.writeByte((byte)id);
        packet.writeInt(++sequence_numbers.key_move_up);
        packet.writeBool(player_attributes.key_move_up);
        return packet;
    }

    public static Packet playerKeyMoveLeft(int id)
    {
        PlayerAttributes player_attributes = GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerAttributes>();
        PlayerSequenceNumbersServer sequence_numbers = GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerSequenceNumbersServer>();

        Packet packet = new Packet(PacketTypeServer.PLAYER_KEY_MOVE_LEFT);
        packet.writeByte((byte)id);
        packet.writeInt(++sequence_numbers.key_move_left);
        packet.writeBool(player_attributes.key_move_left);
        return packet;
    }

    public static Packet playerKeyMoveDown(int id)
    {
        PlayerAttributes player_attributes = GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerAttributes>();
        PlayerSequenceNumbersServer sequence_numbers = GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerSequenceNumbersServer>();

        Packet packet = new Packet(PacketTypeServer.PLAYER_KEY_MOVE_DOWN);
        packet.writeByte((byte)id);
        packet.writeInt(++sequence_numbers.key_move_down);
        packet.writeBool(player_attributes.key_move_down);
        return packet;
    }

    public static Packet playerKeyMoveRight(int id)
    {
        PlayerAttributes player_attributes = GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerAttributes>();
        PlayerSequenceNumbersServer sequence_numbers = GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerSequenceNumbersServer>();

        Packet packet = new Packet(PacketTypeServer.PLAYER_KEY_MOVE_RIGHT);
        packet.writeByte((byte)id);
        packet.writeInt(++sequence_numbers.key_move_right);
        packet.writeBool(player_attributes.key_move_right);
        return packet;
    }

    public static Packet playerKeyDodge(int id)
    {
        PlayerAttributes player_attributes = GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerAttributes>();
        PlayerSequenceNumbersServer sequence_numbers = GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerSequenceNumbersServer>();

        Packet packet = new Packet(PacketTypeServer.PLAYER_KEY_DODGE);
        packet.writeByte((byte)id);
        packet.writeInt(++sequence_numbers.key_dodge);
        packet.writeBool(player_attributes.key_dodge);
        return packet;
    }


    public static Packet playerReady(int id)
    {
        PlayerAttributes player_attributes = GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerAttributes>();
        PlayerSequenceNumbersServer sequence_numbers = GameObject.Find("Game").GetComponent<Game>().players[id].GetComponent<PlayerSequenceNumbersServer>();
        Packet packet = new Packet(PacketTypeServer.PLAYER_READY);
        packet.writeByte((byte)id);
        packet.writeInt(++sequence_numbers.ready);
        packet.writeBool(player_attributes.ready);
        return packet;
    }

    public static Packet gameState(int id)
    {
        Packet packet = new Packet(PacketTypeServer.GAME_STATE);
        packet.writeByte((byte)id);
        return packet;
    }

    public static Packet projectileJoin(int projectile_id, int player_id, Vector3 position)
    {
        Packet packet = new Packet(PacketTypeServer.PROJECTILE_JOIN);
        packet.writeByte((byte)projectile_id);
        packet.writeByte((byte)player_id);
        packet.writeVector3(position);
        return packet;
    }

    public static Packet projectilePosition(int id)
    {
        ProjectileAttributes projectile_attributes = GameObject.Find("Game").GetComponent<Game>().projectiles[id].GetComponent<ProjectileAttributes>();
        ProjectileSequenceNumbers sequence_numbers = GameObject.Find("Game").GetComponent<Game>().projectiles[id].GetComponent<ProjectileSequenceNumbers>();

        Packet packet = new Packet(PacketTypeServer.PROJECTILE_POSITION);
        packet.writeByte((byte)id);
        packet.writeInt(++sequence_numbers.position);
        packet.writeVector3(projectile_attributes.transform.position);
        return packet;
    }

    public static Packet projectileLeave(int id)
    {
        Packet packet = new Packet(PacketTypeServer.PROJECTILE_LEAVE);
        packet.writeByte((byte)id);
        return packet;
    }

    public static Packet boostpacketJoin(int id)
    {
        BoostpacketAttributes boostpacket_attributes = GameObject.Find("Game").GetComponent<Game>().boostpackets[id].GetComponent<BoostpacketAttributes>();
        Packet packet = new Packet(PacketTypeServer.BOOSTPACKET_JOIN);
        packet.writeByte((byte)id);
        packet.writeByte((byte)boostpacket_attributes.type);
        return packet;
    }

    public static Packet boostpacketPosition(int id)
    {
        BoostpacketAttributes boostpacket_attributes = GameObject.Find("Game").GetComponent<Game>().boostpackets[id].GetComponent<BoostpacketAttributes>();
        BoostpacketSequenceNumbers sequence_numbers = GameObject.Find("Game").GetComponent<Game>().boostpackets[id].GetComponent<BoostpacketSequenceNumbers>();

        Packet packet = new Packet(PacketTypeServer.BOOSTPACKET_POSITION);
        packet.writeByte((byte)id);
        packet.writeInt(++sequence_numbers.position);
        packet.writeVector3(boostpacket_attributes.transform.position);
        return packet;
    }

    public static Packet boostpacketRotation(int id)
    {
        BoostpacketAttributes boostpacket_attributes = GameObject.Find("Game").GetComponent<Game>().boostpackets[id].GetComponent<BoostpacketAttributes>();
        BoostpacketSequenceNumbers sequence_numbers = GameObject.Find("Game").GetComponent<Game>().boostpackets[id].GetComponent<BoostpacketSequenceNumbers>();

        Packet packet = new Packet(PacketTypeServer.BOOSTPACKET_ROTATION);
        packet.writeByte((byte)id);
        packet.writeInt(++sequence_numbers.rotation);
        packet.writeVector3(boostpacket_attributes.transform.eulerAngles);
        return packet;
    }

    public static Packet boostpacketLeave(int id)
    {
        Packet packet = new Packet(PacketTypeServer.BOOSTPACKET_LEAVE);
        packet.writeByte((byte)id);
        return packet;
    }


    public static Packet clientHost()
    {
        Packet packet = new Packet(PacketTypeServer.CLIENT_HOST);
        packet.writeByte((byte)GameObject.Find("Server").GetComponent<Server>().host_id);
        return packet;
    }

    public static Packet playersUnreadyAll()
    {
        Debug.Log("send players unready");
        Packet packet = new Packet(PacketTypeServer.PLAYERS_UNREADY_ALL);
        return packet;
    }
}
