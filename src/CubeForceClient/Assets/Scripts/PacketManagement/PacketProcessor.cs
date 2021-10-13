using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;




public static class PacketProcessor
{
    public static void messageAcknowledgment(Packet packet)
    {
        int message_id = (int)packet.readByte();
        Client client = GameObject.Find("Client").GetComponent<Client>();
        if (client.isRunningMessage(message_id))
        {
            long ping = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - client.running_reliable_messages[message_id][1];
            Client.handlePing(ping);
            client.removeRunningMessage(message_id);
        }
    }

    public static bool checkValidity(Packet packet)
    {
        if (packet.getAbsolutePacketLength() <= 1)
        {
            return false;
        }
        int length = (int)packet.readByte();
        return length == packet.getAbsolutePacketLength() - 1; 
        // if(length != packet.getAbsolutePacketLength() - 1){
        //     return false;
        // }
        // byte hash = packet.readByte();
        // byte recomputed_hash = packet.computeHashStartingFromReadposition();
        // if(hash != recomputed_hash){
        //     Debug.Log("WRONG HASH");
        // }
        // return hash == recomputed_hash;
    }

    public static void connectionCheck(Packet packet)
    {

    }


    public static void clientDisconnect(Packet packet)
    {
        int id = (int)packet.readByte();

        // process data
        Game game = GameObject.Find("Game").GetComponent<Game>();
        if (game.players[id] != null)
        {
            game.removePlayer(id);
            GameObject.Find("LobbyBoard").GetComponent<LobbyBoard>().should_update = true;
        }
    }

    public static void playersTransform(Packet packet){
        Game game = GameObject.Find("Game").GetComponent<Game>();
        game.GetComponent<UpdateFrequencyTracker>().reset();
        GameSequenceNumbers game_sequence_numbers = game.GetComponent<GameSequenceNumbers>();
        int sequence_number = packet.readInt();
        if(game_sequence_numbers.players_transform < sequence_number){
            game_sequence_numbers.players_transform = sequence_number;
            while(!packet.isFinishedProcessed()){
                int id = packet.readByte();
                Vector3 position = packet.readVector3();
                Vector3 rotation = packet.readVector3();
                if(game.players[id] != null){
                    game.players[id].transform.GetChild(0).GetComponent<PlayerPosition>().updatePosition(position);
                    game.players[id].transform.GetChild(0).GetComponent<PlayerRotation>().updateRotation(Quaternion.Euler(rotation));
                }
            }
        }
        else{
            Debug.Log("SEQUENCE NUMBER TOO LOW");
        }
    }

    public static void projectilesTransform(Packet packet)
    {
        Game game = GameObject.Find("Game").GetComponent<Game>();
        GameSequenceNumbers game_sequence_numbers = game.GetComponent<GameSequenceNumbers>();
        int sequence_number = packet.readInt();
        if(game_sequence_numbers.projectiles_transform < sequence_number){
            game_sequence_numbers.projectiles_transform = sequence_number;
            while(!packet.isFinishedProcessed()){
                int id = packet.readByte();
                Vector3 position = packet.readVector3();
                if(game.projectiles.ContainsKey(id)){
                    game.projectiles[id].GetComponent<ProjectilePosition>().updatePosition(position);
                }
            }
        }
    }

    public static void boostpacketsTransform(Packet packet){
        Game game = GameObject.Find("Game").GetComponent<Game>();
        GameSequenceNumbers game_sequence_numbers = game.GetComponent<GameSequenceNumbers>();
        int sequence_number = packet.readInt();
        if(game_sequence_numbers.boostpackets_transform < sequence_number){
            game_sequence_numbers.boostpackets_transform = sequence_number;
            while(!packet.isFinishedProcessed()){
                int id = packet.readByte();
                Vector3 position = packet.readVector3();
                Vector3 rotation = packet.readVector3();
                if(game.boostpackets.ContainsKey(id)){
                    game.boostpackets[id].GetComponent<BoostpacketPosition>().updatePosition(position);
                    game.boostpackets[id].GetComponent<BoostpacketRotation>().updateRotation(Quaternion.Euler(rotation));
                }
            }
        }
    }

    public static void playerPosition(Packet packet)
    {
        int id = (int)packet.readByte();
        int sequence_number = packet.readInt();

        Game game = GameObject.Find("Game").GetComponent<Game>();
        if (game.players[id] != null)
        {
            PlayerSequenceNumbersServer sequence_numbers = game.players[id].transform.GetChild(0).GetComponent<PlayerSequenceNumbersServer>();
            if (sequence_number > sequence_numbers.position)
            {
                if (id == Client.id)
                {
                    game.GetComponent<UpdateFrequencyTracker>().reset();
                }
                sequence_numbers.position = sequence_number;
                Vector3 position = packet.readVector3();
                game.players[id].transform.GetChild(0).GetComponent<PlayerPosition>().updatePosition(position);
            }
        }
    }

    public static void playerRotation(Packet packet)
    {
        int id = (int)packet.readByte();
        int sequence_number = packet.readInt();

        Game game = GameObject.Find("Game").GetComponent<Game>();
        if (game.players[id] != null)
        {
            PlayerSequenceNumbersServer sequence_numbers = game.players[id].transform.GetChild(0).GetComponent<PlayerSequenceNumbersServer>();
            if (sequence_number > sequence_numbers.rotation)
            {
                sequence_numbers.rotation = sequence_number;
                Vector3 rotation = packet.readVector3();

                // process data
                game.players[id].transform.GetChild(0).GetComponent<PlayerRotation>().updateRotation(Quaternion.Euler(rotation));
            }
        }
    }

    public static void playerAlive(Packet packet)
    {
        int id = (int)packet.readByte();
        int sequence_number = packet.readInt();

        Game game = GameObject.Find("Game").GetComponent<Game>();
        if (game.players[id] != null)
        {
            PlayerSequenceNumbersServer sequence_numbers = game.players[id].transform.GetChild(0).GetComponent<PlayerSequenceNumbersServer>();
            if (sequence_number > sequence_numbers.alive)
            {
                sequence_numbers.alive = sequence_number;
                bool alive = packet.readBool();
                // process data

                game.players[id].transform.GetChild(0).GetComponent<PlayerAttributes>().alive = alive;
            }
        }
    }


    public static void playerOnFire(Packet packet)
    {
        int id = (int)packet.readByte();
        int sequence_number = packet.readInt();

        Game game = GameObject.Find("Game").GetComponent<Game>();
        if (game.players[id] != null)
        {
            PlayerSequenceNumbersServer sequence_numbers = game.players[id].transform.GetChild(0).GetComponent<PlayerSequenceNumbersServer>();
            if (sequence_number > sequence_numbers.on_fire)
            {
                sequence_numbers.on_fire = sequence_number;
                // process data
                game.players[id].transform.GetChild(0).GetComponent<PlayerAttributes>().on_fire = packet.readBool();
            }
        }
    }

    public static void playerOnSpeed(Packet packet)
    {
        int id = (int)packet.readByte();
        int sequence_number = packet.readInt();
        Game game = GameObject.Find("Game").GetComponent<Game>();
        if (game.players[id] != null)
        {
            PlayerSequenceNumbersServer sequence_numbers = game.players[id].transform.GetChild(0).GetComponent<PlayerSequenceNumbersServer>();
            if (sequence_number > sequence_numbers.on_speed)
            {
                sequence_numbers.on_speed = sequence_number;
                // process data
                game.players[id].transform.GetChild(0).GetComponent<PlayerAttributes>().on_speed = packet.readBool();
            }
        }
    }

    public static void playerMunition(Packet packet)
    {
        int id = (int)packet.readByte();
        int sequence_number = packet.readInt();

        Game game = GameObject.Find("Game").GetComponent<Game>();
        if (game.players[id] != null)
        {
            PlayerSequenceNumbersServer sequence_numbers = game.players[id].transform.GetChild(0).GetComponent<PlayerSequenceNumbersServer>();
            if (sequence_number > sequence_numbers.munition)
            {
                sequence_numbers.munition = sequence_number;
                int munition = packet.readByte();
                game.players[id].transform.GetChild(0).GetComponent<PlayerAttributes>().munition = munition;
                GameObject.Find("Munition")?.GetComponent<MunitionUI>().updateMunitionUI(munition, game.materials[id]);
            }
        }
    }

    public static void playerSideStepReady(Packet packet)
    {
        int id = (int)packet.readByte();
        int sequence_number = packet.readInt();

        Game game = GameObject.Find("Game").GetComponent<Game>();
        if (game.players[id] != null)
        {
            PlayerSequenceNumbersServer sequence_numbers = game.players[id].transform.GetChild(0).GetComponent<PlayerSequenceNumbersServer>();
            if (sequence_number > sequence_numbers.sidestep_ready)
            {
                sequence_numbers.sidestep_ready = sequence_number;
                bool sidestep_ready = packet.readBool();

                // process data

                game.players[id].transform.GetChild(0).GetComponent<PlayerAttributes>().sidestep_ready = sidestep_ready;
            }
        }
    }

    public static void playerShieldReady(Packet packet)
    {
        int id = (int)packet.readByte();
        int sequence_number = packet.readInt();

        Game game = GameObject.Find("Game").GetComponent<Game>();
        if (game.players[id] != null)
        {
            PlayerSequenceNumbersServer sequence_numbers = game.players[id].transform.GetChild(0).GetComponent<PlayerSequenceNumbersServer>();
            if (sequence_number > sequence_numbers.shield_ready)
            {
                sequence_numbers.shield_ready = sequence_number;
                bool shield_ready = packet.readBool();

                // process data

                game.players[id].transform.GetChild(0).GetComponent<PlayerAttributes>().shield_ready = shield_ready;
                GameObject.Find("ShieldCooldown")?.GetComponent<ShieldUI>().updateShield(shield_ready);
            }
        }
    }

    public static void playerShieldActive(Packet packet)
    {
        int id = (int)packet.readByte();
        int sequence_number = packet.readInt();

        Game game = GameObject.Find("Game").GetComponent<Game>();
        if (game.players[id] != null)
        {
            PlayerSequenceNumbersServer sequence_numbers = game.players[id].transform.GetChild(0).GetComponent<PlayerSequenceNumbersServer>();
            if (sequence_number > sequence_numbers.shield_active)
            {
                sequence_numbers.shield_active = sequence_number;
                bool shield_active = packet.readBool();

                // process data

                game.players[id].transform.GetChild(0).GetComponent<PlayerAttributes>().shield_active = shield_active;
            }
        }
    }

    public static void playerKills(Packet packet)
    {
        int id = (int)packet.readByte();
        int sequence_number = packet.readInt();

        Game game = GameObject.Find("Game").GetComponent<Game>();
        if (game.players[id] != null)
        {
            PlayerSequenceNumbersServer sequence_numbers = game.players[id].transform.GetChild(0).GetComponent<PlayerSequenceNumbersServer>();
            if (sequence_number > sequence_numbers.kills)
            {
                sequence_numbers.kills = sequence_number;
                int kills = packet.readInt();

                // process data

                game.players[id].transform.GetChild(0).GetComponent<PlayerAttributes>().kills = kills;
            }
        }
    }

    public static void playerWins(Packet packet)
    {
        int id = (int)packet.readByte();
        int sequence_number = packet.readInt();

        Game game = GameObject.Find("Game").GetComponent<Game>();
        if (game.players[id] != null)
        {
            PlayerSequenceNumbersServer sequence_numbers = game.players[id].transform.GetChild(0).GetComponent<PlayerSequenceNumbersServer>();
            if (sequence_number > sequence_numbers.wins)
            {
                sequence_numbers.wins = sequence_number;
                int wins = packet.readInt();

                // process data
                game.players[id].transform.GetChild(0).GetComponent<PlayerAttributes>().wins = wins;
            }
        }
    }

    public static void playerKeys(Packet packet)
    { //wasd
        int id = (int)packet.readByte();
        int sequence_number = packet.readInt();

        Game game = GameObject.Find("Game").GetComponent<Game>();
        if (game.players[id] != null)
        {
            PlayerSequenceNumbersServer sequence_numbers = game.players[id].transform.GetChild(0).GetComponent<PlayerSequenceNumbersServer>();
            if (sequence_number > sequence_numbers.keys)
            {
                sequence_numbers.keys = sequence_number;
                bool key_move_up = packet.readBool();
                bool key_move_left = packet.readBool();
                bool key_move_down = packet.readBool();
                bool key_move_right = packet.readBool();

                // process data

                game.players[id].transform.GetChild(0).GetComponent<PlayerAttributes>().key_move_up = key_move_up;
                game.players[id].transform.GetChild(0).GetComponent<PlayerAttributes>().key_move_left = key_move_left;
                game.players[id].transform.GetChild(0).GetComponent<PlayerAttributes>().key_move_down = key_move_down;
                game.players[id].transform.GetChild(0).GetComponent<PlayerAttributes>().key_move_right = key_move_right;
            }
        }
    }

    public static void playerKeyMoveUp(Packet packet)
    {
        int id = (int)packet.readByte();
        int sequence_number = packet.readInt();

        Game game = GameObject.Find("Game").GetComponent<Game>();
        if (game.players[id] != null)
        {
            PlayerSequenceNumbersServer sequence_numbers = game.players[id].transform.GetChild(0).GetComponent<PlayerSequenceNumbersServer>();
            if (sequence_number > sequence_numbers.key_move_up)
            {
                sequence_numbers.key_move_up = sequence_number;
                game.players[id].transform.GetChild(0).GetComponent<PlayerAttributes>().key_move_up = packet.readBool();
            }
        }
    }

    public static void playerKeyMoveLeft(Packet packet)
    {
        int id = (int)packet.readByte();
        int sequence_number = packet.readInt();

        Game game = GameObject.Find("Game").GetComponent<Game>();
        if (game.players[id] != null)
        {
            PlayerSequenceNumbersServer sequence_numbers = game.players[id].transform.GetChild(0).GetComponent<PlayerSequenceNumbersServer>();
            if (sequence_number > sequence_numbers.key_move_left)
            {
                sequence_numbers.key_move_left = sequence_number;
                game.players[id].transform.GetChild(0).GetComponent<PlayerAttributes>().key_move_left = packet.readBool();
            }
        }
    }

    public static void playerKeyMoveDown(Packet packet)
    {
        int id = (int)packet.readByte();
        int sequence_number = packet.readInt();

        Game game = GameObject.Find("Game").GetComponent<Game>();
        if (game.players[id] != null)
        {
            PlayerSequenceNumbersServer sequence_numbers = game.players[id].transform.GetChild(0).GetComponent<PlayerSequenceNumbersServer>();
            if (sequence_number > sequence_numbers.key_move_down)
            {
                sequence_numbers.key_move_down = sequence_number;
                game.players[id].transform.GetChild(0).GetComponent<PlayerAttributes>().key_move_down = packet.readBool();
            }
        }
    }

    public static void playerKeyMoveRight(Packet packet)
    {
        int id = (int)packet.readByte();
        int sequence_number = packet.readInt();

        Game game = GameObject.Find("Game").GetComponent<Game>();
        if (game.players[id] != null)
        {
            PlayerSequenceNumbersServer sequence_numbers = game.players[id].transform.GetChild(0).GetComponent<PlayerSequenceNumbersServer>();
            if (sequence_number > sequence_numbers.key_move_right)
            {
                sequence_numbers.key_move_right = sequence_number;
                game.players[id].transform.GetChild(0).GetComponent<PlayerAttributes>().key_move_right = packet.readBool();
            }
        }
    }

    public static void playerKeyDodge(Packet packet)
    {
        int id = (int)packet.readByte();
        int sequence_number = packet.readInt();

        Game game = GameObject.Find("Game").GetComponent<Game>();
        if (game.players[id] != null)
        {
            PlayerSequenceNumbersServer sequence_numbers = game.players[id].transform.GetChild(0).GetComponent<PlayerSequenceNumbersServer>();
            if (sequence_number > sequence_numbers.key_dodge)
            {
                sequence_numbers.key_dodge = sequence_number;
                game.players[id].transform.GetChild(0).GetComponent<PlayerAttributes>().key_dodge = packet.readBool();
            }
        }
    }

    public static void playerReady(Packet packet)
    {
        int id = (int)packet.readByte();
        int sequence_number = packet.readInt();

        Game game = GameObject.Find("Game").GetComponent<Game>();
        if (game.players[id] != null)
        {
            PlayerSequenceNumbersServer sequence_numbers = game.players[id].transform.GetChild(0).GetComponent<PlayerSequenceNumbersServer>();
            if (sequence_number > sequence_numbers.ready)
            {
                sequence_numbers.ready = sequence_number;
                game.players[id].transform.GetChild(0).GetComponent<PlayerAttributes>().ready = packet.readBool();
                GameObject.Find("LobbyBoard").GetComponent<LobbyBoard>().should_update = true;
            }
        }
    }

    public static void gameState(Packet packet)
    {
        int state_id = (int)packet.readByte();

        //process data
        Game game = GameObject.Find("Game").GetComponent<Game>();
        if ((int)game.getGameStateID() != state_id)
        {
            game.changeStateTo(game.states[state_id]);
        }
    }

    public static void projectileJoin(Packet packet)
    {
        int projectile_id = (int)packet.readByte();
        int player_id = (int)packet.readByte();
        Vector3 position = packet.readVector3();

        // process data
        Game game = GameObject.Find("Game").GetComponent<Game>();
        if (!game.projectiles.ContainsKey(projectile_id))
        {
            game.createProjectile(projectile_id, player_id, position);
        }
    }

    public static void projectilePosition(Packet packet)
    {
        int id = (int)packet.readByte();
        int sequence_number = packet.readInt();

        Game game = GameObject.Find("Game").GetComponent<Game>();
        if (game.projectiles.ContainsKey(id))
        {
            ProjectileSequenceNumbers sequence_numbers = game.projectiles[id].GetComponent<ProjectileSequenceNumbers>();

            if (sequence_number > sequence_numbers.position)
            {
                sequence_numbers.position = sequence_number;
                Vector3 position = packet.readVector3();

                // process data
                //game.projectiles[id].GetComponent<ProjectileAttributes>().transform.position = position;
                game.projectiles[id].GetComponent<ProjectilePosition>().updatePosition(position);
            }
        }
    }

    public static void projectileLeave(Packet packet)
    {
        int id = (int)packet.readByte();

        // process data
        Game game = GameObject.Find("Game").GetComponent<Game>();
        if (game.projectiles.ContainsKey(id))
        {
            game.removeProjectile(id);
        }
    }

    public static void boostpacketJoin(Packet packet)
    {
        int id = (int)packet.readByte();
        int type = (int)packet.readByte();
        // process data
        Game game = GameObject.Find("Game").GetComponent<Game>();
        if (!game.boostpackets.ContainsKey(id))
        {
            game.createBoostpacket(id, type);
        }
    }

    public static void boostpacketPosition(Packet packet)
    {
        int id = (int)packet.readByte();
        int sequence_number = (int)packet.readInt();

        Game game = GameObject.Find("Game").GetComponent<Game>();
        if (game.boostpackets.ContainsKey(id))
        {
            BoostpacketSequenceNumbers sequence_numbers = game.boostpackets[id].GetComponent<BoostpacketSequenceNumbers>();

            if (sequence_number > sequence_numbers.position)
            {
                sequence_numbers.position = sequence_number;

                Vector3 position = packet.readVector3();
                //game.boostpackets[id].transform.position = position;
                game.boostpackets[id].GetComponent<BoostpacketPosition>().updatePosition(position);
            }
        }
    }

    public static void boostpacketRotation(Packet packet)
    {
        int id = (int)packet.readByte();
        int sequence_number = (int)packet.readInt();

        Game game = GameObject.Find("Game").GetComponent<Game>();
        if (game.boostpackets.ContainsKey(id))
        {
            BoostpacketSequenceNumbers sequence_numbers = game.boostpackets[id].GetComponent<BoostpacketSequenceNumbers>();
            if (sequence_number > sequence_numbers.rotation)
            {
                sequence_numbers.rotation = sequence_number;

                Vector3 rotation = packet.readVector3();
                //game.boostpackets[id].transform.eulerAngles = rotation;
                game.boostpackets[id].GetComponent<BoostpacketRotation>().updateRotation(Quaternion.Euler(rotation));
            }
        }
    }

    public static void boostpacketLeave(Packet packet)
    {
        int id = (int)packet.readByte();
        Game game = GameObject.Find("Game").GetComponent<Game>();
        if (game.boostpackets.ContainsKey(id))
        {
            game.removeBoostpacket(id);
        }
    }


    public static void clientAdmissionConfirmation(Packet packet)
    {
        int id = (int)packet.readByte();
        if (Client.id == -1)
        {
            Client.id = id;
            ThreadManager.ExecuteOnMainThread(() =>
            {
                Packet packet_response = PacketManufacturer.clientAdmissionConfirmation(Client.id);
                GameObject.Find("Client").GetComponent<Client>().sendMessageReliable(packet_response);
                GameObject.Find("Client").GetComponent<Client>().enterGame();
            });
        }
    }

    public static void playerIntroduction(Packet packet)
    {
        int id = (int)packet.readByte();
        Vector3 position = packet.readVector3();
        string name = packet.readString();

        // process data
        Game game = GameObject.Find("Game").GetComponent<Game>();
        if (game.players[id] == null)
        {
            game.createPlayer(id, name, position);
            GameObject.Find("LobbyBoard").GetComponent<LobbyBoard>().should_update = true;
        }
    }

    public static void clientHost(Packet packet)
    {
        int id = (int)packet.readByte();
        Client.host_id = id;
        GameObject.Find("LobbyBoard").GetComponent<LobbyBoard>().should_update = true;
    }

    public static void playersUnreadyAll()
    {
        Game game = GameObject.Find("Game").GetComponent<Game>();
        foreach (GameObject player in game.players)
        {
            if (player != null)
            {
                player.transform.GetChild(0).GetComponent<PlayerAttributes>().ready = false;
            }
        }
    }

    public static void playerTransform(Packet packet){
        int id = packet.readByte();
        int sequence_number = packet.readInt();
        Vector3 position = packet.readVector3();
        Vector3 rotation = packet.readVector3();
        Game game = GameObject.Find("Game").GetComponent<Game>();
        if (game.players[id] != null)
        {
            PlayerSequenceNumbersServer sequence_numbers = game.players[id].transform.GetChild(0).GetComponent<PlayerSequenceNumbersServer>();
            if(sequence_number > sequence_numbers.player_transform){
                if (id == Client.id){
                    game.GetComponent<UpdateFrequencyTracker>().reset();
                }
                sequence_numbers.position = sequence_number;
                game.players[id].transform.GetChild(0).GetComponent<PlayerPosition>().updatePosition(position);
                game.players[id].transform.GetChild(0).GetComponent<PlayerRotation>().updateRotation(Quaternion.Euler(rotation));
            }
        }
    }


    public static void processPacket(Packet packet, PacketTypeServer packet_type_server)
    {
        // not-only main thread processing methods:
        switch (packet_type_server)
        {
            case PacketTypeServer.CLIENT_ADMISSION_CONFIRMATION:
                clientAdmissionConfirmation(packet);
                break;
            default:
                break;
        }

        // only main thread processing methods
        ThreadManager.ExecuteOnMainThread(() =>
        {
            Game game = GameObject.Find("Game").GetComponent<Game>();
            switch (packet_type_server)
            {
                case PacketTypeServer.PLAYER_POSITION:
                    playerPosition(packet);
                    break;
                case PacketTypeServer.PLAYER_ROTATION:
                    playerRotation(packet);
                    break;
                case PacketTypeServer.PLAYER_ALIVE:
                    playerAlive(packet);
                    break;
                case PacketTypeServer.PLAYER_ON_FIRE:
                    playerOnFire(packet);
                    break;
                case PacketTypeServer.PLAYER_MUNITION:
                    playerMunition(packet);
                    break;
                case PacketTypeServer.PLAYER_SIDESTEP_READY:
                    playerSideStepReady(packet);
                    break;
                case PacketTypeServer.PLAYER_SHIELD_READY:
                    playerShieldReady(packet);
                    break;
                case PacketTypeServer.PLAYER_SHIELD_ACTIVE:
                    playerShieldActive(packet);
                    break;
                case PacketTypeServer.PLAYER_KILLS:
                    playerKills(packet);
                    break;
                case PacketTypeServer.PLAYER_WINS:
                    playerWins(packet);
                    break;
                case PacketTypeServer.PLAYER_KEYS:
                    playerKeys(packet);
                    break;
                case PacketTypeServer.PROJECTILE_JOIN:
                    projectileJoin(packet);
                    break;
                case PacketTypeServer.PROJECTILE_LEAVE:
                    projectileLeave(packet);
                    break;
                case PacketTypeServer.PROJECTILE_POSITION:
                    projectilePosition(packet);
                    break;
                case PacketTypeServer.BOOSTPACKET_JOIN:
                    boostpacketJoin(packet);
                    break;
                case PacketTypeServer.BOOSTPACKET_POSITION:
                    boostpacketPosition(packet);
                    break;
                case PacketTypeServer.BOOSTPACKET_ROTATION:
                    boostpacketRotation(packet);
                    break;
                case PacketTypeServer.BOOSTPACKET_LEAVE:
                    boostpacketLeave(packet);
                    break;
                case PacketTypeServer.CLIENT_DISCONNECT:
                    clientDisconnect(packet);
                    break;
                case PacketTypeServer.GAME_STATE:
                    gameState(packet);
                    break;
                case PacketTypeServer.CONNECTION_CHECK:
                    connectionCheck(packet);
                    break;
                // case PacketTypeServer.CLIENT_ADMISSION_CONFIRMATION:
                //     clientAdmissionConfirmation(packet);
                //     break;
                case PacketTypeServer.PLAYER_INTRODUCTION:
                    playerIntroduction(packet);
                    break;
                case PacketTypeServer.MESSAGE_ACKNOWLEDGMENT:
                    messageAcknowledgment(packet);
                    break;
                case PacketTypeServer.PLAYER_READY:
                    playerReady(packet);
                    break;
                case PacketTypeServer.CLIENT_HOST:
                    clientHost(packet);
                    break;
                case PacketTypeServer.PLAYER_KEY_MOVE_UP:
                    playerKeyMoveUp(packet);
                    break;
                case PacketTypeServer.PLAYER_KEY_MOVE_LEFT:
                    playerKeyMoveLeft(packet);
                    break;
                case PacketTypeServer.PLAYER_KEY_MOVE_DOWN:
                    playerKeyMoveDown(packet);
                    break;
                case PacketTypeServer.PLAYER_KEY_MOVE_RIGHT:
                    playerKeyMoveRight(packet);
                    break;
                case PacketTypeServer.PLAYER_KEY_DODGE:
                    playerKeyDodge(packet);
                    break;
                case PacketTypeServer.PLAYERS_UNREADY_ALL:
                    playersUnreadyAll();
                    break;
                case PacketTypeServer.PLAYER_ON_SPEED:
                    playerOnSpeed(packet);
                    break;   
                // case PacketTypeServer.PLAYERS_TRANSFORM:
                //     playersTransform(packet);
                //     break;
                case PacketTypeServer.PROJECTILES_TRANSFORM:
                    projectilesTransform(packet);
                    break;
                case PacketTypeServer.BOOSTPACKETS_TRANSFORM:
                    boostpacketsTransform(packet);
                    break;
                case PacketTypeServer.PLAYER_TRANSFORM:
                    playerTransform(packet);
                    break;
                default:
                    break;
            }
        });
    }




}
