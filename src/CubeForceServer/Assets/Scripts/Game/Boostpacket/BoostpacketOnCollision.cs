using UnityEngine;
using System.Collections;


public enum BoostpacketType
{
    MUNITION,
    ON_FIRE,
    ON_SPEED
}

public class BoostpacketOnCollision : MonoBehaviour
{

    private Server server;

    void Start()
    {
        server = GameObject.Find("Server").GetComponent<Server>();
        //StartCoroutine("send");
    }

    private IEnumerator send()
    {
        Packet packet = PacketManufacturer.boostpacketPosition(GetComponent<BoostpacketAttributes>().id);
        server.sendMessageBroadcast(packet);

        packet = PacketManufacturer.boostpacketRotation(GetComponent<BoostpacketAttributes>().id);
        server.sendMessageBroadcast(packet);
        yield return new WaitForSeconds(server.send_interval);
        StartCoroutine("send");
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerAttributes player_attributes = collision.gameObject.GetComponent<PlayerAttributes>();
            Packet packet;
            switch (GetComponent<BoostpacketAttributes>().type)
            {
                case BoostpacketType.MUNITION:
                    player_attributes.munition = GameObject.Find("Game").GetComponent<GameAttributes>().munition_boost;
                    // inform respective player about his munition count
                    packet = PacketManufacturer.playerMunition(player_attributes.id);
                    server.sendMessageReliable(player_attributes.id, packet);
                    break;
                case BoostpacketType.ON_SPEED:
                    player_attributes.startOnSpeed();
                    packet = PacketManufacturer.playerOnSpeed(player_attributes.id);
                    server.sendMessageBroadcastReliable(packet);
                    break;
                case BoostpacketType.ON_FIRE:
                    player_attributes.startOnFire();
                    packet = PacketManufacturer.playerOnFire(player_attributes.id);
                    server.sendMessageBroadcastReliable(packet);
                    break;
            }

            GameObject.Find("Game").GetComponent<Game>().removeBoostPacket(GetComponent<BoostpacketAttributes>().id);
        }
    }
}