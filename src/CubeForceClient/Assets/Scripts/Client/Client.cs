using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using System.Collections;

public class Client : MonoBehaviour{

    public GameObject input_ip;
    public GameObject input_name;
    public GameObject login_screen;

    public static int id = -1;

    const int udp_port_num_local = 60001;
    const int udp_port_num_server = 60002;


    public static UdpClient udp_client {get; private set;}

    public static List<long> pings = new List<long>();

    private Game game;

    public Dictionary<int,long[]> running_reliable_messages = new Dictionary<int,long[]>(); // <id, count>

    public static int host_id;

    public void Awake(){
        game = GameObject.Find("Game").GetComponent<Game>();
    }

    public void Update(){
        ThreadManager.UpdateMain();
    }

    public static string server_ip;
    public static string player_name;

    public static IPEndPoint ip_endpoint;


    public void tryConnect(){
        server_ip = input_ip.GetComponent<TMP_InputField>().text;
        player_name = input_name.GetComponent<TMP_InputField>().text;

        if(udp_client != null){
            close();
        }
        IPEndPoint local_end_point = new IPEndPoint(IPAddress.Any, udp_port_num_local);
        udp_client = new UdpClient(local_end_point);

        ip_endpoint = new IPEndPoint(IPAddress.Parse(server_ip), udp_port_num_server);
        
        try{
            udp_client.BeginReceive(udpReceiveMessage,null);

            Packet packet = PacketManufacturer.clientIntroduction(player_name);
            sendMessageReliable(packet);
        }
        catch(Exception e){
            Debug.Log(e);
            Debug.Log("Wrong Server address or name too long (max 7 letters)");
        }
    }
    

    public void enterGame(){
        Destroy(login_screen);

        GameObject.Find("Game").GetComponent<Game>().start();
    }


    public void sendMessage(Packet packet){
        if(Client.id != -1){
            byte[] msg = packet.getWrappedPacket();
            udp_client.BeginSend(msg,msg.Length,ip_endpoint,null,null);
        }
    }

    public void sendMessageReliable(Packet packet){
        int msg_id = getUnusedRunningMessageID();
        byte[] msg = packet.getWrappedPacketReliable(msg_id);
        
        addRunningMessage(msg_id);
        StartCoroutine(resendMessageReliable(msg_id,msg));
    }

    public int getUnusedRunningMessageID(){
        for(int i = 0; i < 255; i++){
            if(!running_reliable_messages.ContainsKey(i)){
                return i;
            }
        }
        Debug.LogError("No free runninge message id found");
        return -1;
    }

    public IEnumerator resendMessageReliable(int message_id, byte[] packet){
        if(isRunningMessage(message_id)){
            if(isRunningMessageExpired(message_id)){
                // remove client
                close();
                Debug.Log("running message expired");
            }
            else{
                udp_client.BeginSend(packet,packet.Length,ip_endpoint,null,null);
                running_reliable_messages[message_id][0]++;
                yield return new WaitForSeconds(0.1f);
                StartCoroutine(resendMessageReliable(message_id,packet));
            }
        }
        // if the message id is not in the dictionary anymore, it means that the client has responded already
    }

    public void addRunningMessage(int message_id){
        running_reliable_messages.Add(message_id,new long[2]{0,DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond});
    }

    public void removeRunningMessage(int message_id){
        running_reliable_messages.Remove(message_id);
    }

    public bool isRunningMessage(int message_id){
        return running_reliable_messages.ContainsKey(message_id);
    }

    public bool isRunningMessageExpired(int message_id){
        return running_reliable_messages[message_id][0] > 20;
    }



    public static void udpReceiveMessage(IAsyncResult _result){
        try{
            if(udp_client == null) return;
            IPEndPoint ep = new IPEndPoint(IPAddress.Any,0);

            byte[] data = udp_client.EndReceive(_result,ref ep);
            udp_client.BeginReceive(udpReceiveMessage,null);

            Packet packet = new Packet(data);
            if(PacketProcessor.checkValidity(packet)){
                PacketTypeServer packet_type_server = (PacketTypeServer)packet.readByte();
                if(packet_type_server == PacketTypeServer.RELIABLE){
                    // if the message was meant to be reliable, then directly answer the sender client with an acknowledgement message
                    int msg_id = (int)packet.readByte();

                    packet_type_server = (PacketTypeServer)packet.readByte();
                    PacketProcessor.processPacket(packet,packet_type_server);

                    Packet new_packet = PacketManufacturer.messageAcknowledgment(Client.id,msg_id);
                    byte[] msg = new_packet.getWrappedPacket();
                    udp_client.BeginSend(msg,msg.Length,ip_endpoint,null,null); 
                }
                else{
                    PacketProcessor.processPacket(packet,packet_type_server);
                }
            }
        }
        catch(Exception e){
            Debug.Log("Smth wrong happened at receiving udp");
            Debug.Log(e);
        }
    }


    public static void handlePing(long ping){
        if(pings.Count >= 10){
            pings.RemoveAt(0);
        }
        pings.Add(ping);
    }

    public static long getPingInMilliseconds(){
        long sum = 0;
        foreach(long ping in pings){
            sum += ping;
        }
        return sum/pings.Count;
    }



    void OnApplicationQuit(){
        close();
        Debug.Log("closed everything");
    }

    public static void close(){
        if(udp_client != null){
            udp_client.Close();
            udp_client = null;
            id = -1;
        }
    }


}