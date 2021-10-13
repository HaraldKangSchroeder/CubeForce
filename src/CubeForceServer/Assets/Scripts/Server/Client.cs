using System.Net;
using System.Collections.Generic;
using UnityEngine;


public class Client{

    public IPEndPoint ip_endpoint {get; private set;}
    public int id {get; private set;}
    public string name;


    public Dictionary<int,int> running_reliable_messages = new Dictionary<int,int>(); // <id, count> 


    public Client(int id, IPEndPoint ep, string name){
        this.id = id;
        this.ip_endpoint = ep;
        this.name = name;
    }

    public int getUnusedRunningMessageID(){
        for(int i = 0; i < 255; i++){
            if(!running_reliable_messages.ContainsKey(i)){
                return i;
            }
        }
        Debug.LogError("No free running message id found");
        return -1;
    }

    public void addRunningMessage(int msg_id){
        running_reliable_messages.Add(msg_id,0);
    }

    public void removeRunningMessage(int msg_id){
        running_reliable_messages.Remove(msg_id);
    }

    public bool isRunningMessage(int msg_id){
        return running_reliable_messages.ContainsKey(msg_id);
    }

    public bool isRunningMessageExpired(int msg_id){
        return running_reliable_messages[msg_id] > Server.resend_count_until_disconnect;
    }
}