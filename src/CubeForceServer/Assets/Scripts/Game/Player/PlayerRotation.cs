using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    private PlayerAttributes player_attributes;
    private Rigidbody rb;
    private Server server;

    void Start()
    {
        player_attributes = transform.GetComponent<PlayerAttributes>();
        server = GameObject.Find("Server").GetComponent<Server>();
        rb = GetComponent<Rigidbody>();

        //StartCoroutine("send");
    }

    private void FixedUpdate()
    {
        float m_Angle = player_attributes.rotation_angle;
        rb.MoveRotation(Quaternion.Euler(new Vector3(0, m_Angle - 180, 0)));
    }

    private IEnumerator send()
    {
        Packet packet_player_rotation = PacketManufacturer.playerRotation(player_attributes.id);
        server.sendMessageBroadcast(packet_player_rotation);
        yield return new WaitForSeconds(server.send_interval);
        StartCoroutine("send");
    }
}
