using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections;


public class Server : MonoBehaviour{
    
    // port on which the server should run
    public const int port = 60002;

    public int host_id;
    public float send_interval;
    public float resend_delay;
    [HideInInspector] public  Client[] clients;
    public static int resend_count_until_disconnect = 20;
    public static int num_clients = 6;
    private  UdpClient udp_client;



    private void Awake() {
        // Here, if you want to make it accessable from the global network, you cant run this in editor_mode
        // declare clients
        clients = new Client[num_clients];
        IPEndPoint end_point = new IPEndPoint(IPAddress.Any, port);
        udp_client   = new UdpClient(end_point);
        udp_client.Client.ReceiveBufferSize = 65536 * 3;


        // udp creation with IOControl, see https://stackoverflow.com/questions/38191968/c-sharp-udp-an-existing-connection-was-forcibly-closed-by-the-remote-host
        int SIO_UDP_CONNRESET = -1744830452;
        udp_client.Client.IOControl(
            (IOControlCode)SIO_UDP_CONNRESET, 
            new byte[] { 0, 0, 0, 0 }, 
            null
        );

        udp_client.BeginReceive(udpReceiveMessage,null);

    }

    public void sendMessageBroadcast(Packet packet){
        try{
            byte[] msg = packet.getWrappedPacket();
            for(int i = 0; i < clients.Length; i++){
                if(clients[i] != null ){
                    udp_client.BeginSend(msg,msg.Length,clients[i].ip_endpoint,null,null);
                }
            }
        }
        catch(Exception e){
            Debug.LogError(e);
        }
    }


    public void sendMessageBroadcastReliable(Packet packet){
        try{
            for(int i = 0; i < clients.Length; i++){
                if(clients[i] != null ){
                    int msg_id = clients[i].getUnusedRunningMessageID();
                    byte[] msg = packet.getWrappedPacketReliable(msg_id);
                    
                    clients[i].addRunningMessage(msg_id);
                    StartCoroutine(resendMessageReliable(clients[i].id,msg_id,msg));
                }
            }
        }
        catch(Exception e){
            Debug.LogError(e);
        }
    }

    public void sendMessageBroadcastExceptOne(int id, Packet packet){
        try{
            byte[] msg = packet.getWrappedPacket();
            for(int i = 0; i < clients.Length; i++){
                if(clients[i] != null && clients[i].id != id ){
                    udp_client.BeginSend(msg,msg.Length,clients[i].ip_endpoint,null,null);
                }
            }
        }
        catch(Exception e){
            Debug.LogError(e);
        }
    }

    public void sendMessageBroadcastExceptOneReliable(int id, Packet packet){
        try{
            for(int i = 0; i < clients.Length; i++){
                if(clients[i] != null && clients[i].id != id ){
                    int msg_id = clients[i].getUnusedRunningMessageID();
                    byte[] msg = packet.getWrappedPacketReliable(msg_id);
                    
                    clients[i].addRunningMessage(msg_id);
                    StartCoroutine(resendMessageReliable(clients[i].id,msg_id,msg));
                }
            }
        }
        catch(Exception e){
            Debug.LogError(e);
        }
    }



    public void sendMessage(int id, Packet packet){
        if(clients[id] != null ){
            byte[] msg = packet.getWrappedPacket();
            udp_client.BeginSend(msg,msg.Length,clients[id].ip_endpoint,null,null);
        }
    }

    public void sendMessageReliable(int id, Packet packet){
        if(clients[id] != null ){
            int msg_id = clients[id].getUnusedRunningMessageID();
            byte[] msg = packet.getWrappedPacketReliable(msg_id);

            clients[id].addRunningMessage(msg_id);
            StartCoroutine(resendMessageReliable(id,msg_id,msg));
        }
    }


    public IEnumerator resendMessageReliable(int id, int msg_id, byte[] packet){
        if(clients[id] != null){
            if(clients[id].isRunningMessage(msg_id)){
                if(clients[id].isRunningMessageExpired(msg_id)){
                    // remove client
                    Debug.Log("remove client " + id);
                    GameObject.Find("Game").GetComponent<Game>().removePlayer(id);
                    removeDisconnectedClient(clients[id]);
                }
                else{
                    udp_client.BeginSend(packet,packet.Length,clients[id].ip_endpoint,null,null);
                    clients[id].running_reliable_messages[msg_id]++;
                    yield return new WaitForSeconds(resend_delay);
                    StartCoroutine(resendMessageReliable(id,msg_id,packet));
                }
            }
        }
        // if the message id is not in the dictionary anymore, it means that the client has responded already
    }


    public void udpReceiveMessage(IAsyncResult _result){
        try{
            IPEndPoint ep = new IPEndPoint(IPAddress.Any,0);

            byte[] data = udp_client.EndReceive(_result,ref ep);
            udp_client.BeginReceive(udpReceiveMessage,null);

            Packet packet = new Packet(data);
            if(PacketProcessor.checkValidity(packet)){
                PacketTypeClient packet_type_client = (PacketTypeClient)packet.readByte();
                if(packet_type_client == PacketTypeClient.RELIABLE){
                    // if the message was meant to be reliable, then directly answer the sender client with an acknowledgement message
                    int msg_id = (int)packet.readByte();
                    Packet new_packet = PacketManufacturer.messageAcknowledgment(msg_id);
                    byte[] msg = new_packet.getWrappedPacket();
                    udp_client.BeginSend(msg,msg.Length,ep,null,null);
                    
                    packet_type_client = (PacketTypeClient)packet.readByte();
                }
                PacketProcessor.processPacket(packet,packet_type_client,ep);
            }
        }
        catch(Exception e){
            Debug.Log("Smth wrong happened at receiving udp");
            Debug.Log(e);
        }
    }


    public void connectionsCheck(){
        Packet packet = PacketManufacturer.connectionCheck();
        sendMessageBroadcastReliable(packet);
    }


    void OnApplicationQuit()
    {
        udp_client.Close();
    }


    void Update(){
        ThreadManager.UpdateMain();
    }

    




    public bool isClient(IPEndPoint ep){
        for(int i = 0; i < clients.Length; i++){
            if(clients[i] != null){
                if(clients[i].ip_endpoint.Equals(ep)){
                    return true;
                }
            }
        }
        return false;
    }

    public bool isGameFullyOccupied(){
        for(int i = 0; i < clients.Length; i++){
            if(clients[i] == null){
                return false;
            }
        }
        return true;
    }

    public bool isGameInLobby(){
        return GameObject.Find("Game").GetComponent<Game>().getGameStateID() == StateID.LOBBY;
    }

    public int storeClient(IPEndPoint ep,string name){
        Debug.Log("New Tcp Client - New Client detected");

        // compute a new - non-existent id which is used to identify the player
        int client_id = assignID();
        Debug.Log("Assign ID: " + client_id);

        Client client = new Client(client_id, ep, name);
        clients[client_id] = client;

        // if the new client is the only client atm, then he becomes the host as well
        if(getClientsCount() == 1){
            host_id = client_id;
        }

        return client_id;
    }

    public void removeDisconnectedClient(Client client){
        clients[client.id] = null;
        Packet packet_disconnect = PacketManufacturer.clientDisconnect(client.id);
        sendMessageBroadcastReliable(packet_disconnect);
        
        if(host_id == client.id){
            if(getClientsCount() > 0){
                // SET NEW HOST
                for(int i = 0; i < clients.Length; i++){
                    if(clients[i] != null){
                        host_id = clients[i].id;
                    }
                }
                // INFORM ALL
                Packet packet = PacketManufacturer.clientHost();
                sendMessageBroadcastReliable(packet);
            }
            else{
                host_id = -1;
            }
        }
    }

    public int getClientsCount(){
        int sum = 0;
        for(int i = 0; i < clients.Length; i++){
            if(clients[i] != null){
                sum += 1;
            }
        }
        return sum;
    }




    //returns an unique/unused id
    public int assignID(){
        for(int i = 0;i < clients.Length; i++){
            if(clients[i] == null){
                return i;
            }
        }
        return -1;
    }

    



}
