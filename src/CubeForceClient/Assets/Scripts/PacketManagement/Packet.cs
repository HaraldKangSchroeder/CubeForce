using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;



public class Packet
{
    // buffer which gets filled during writing
    private List<byte> buffer;

    // packet that is read
    private byte[] packet;
    private int read_position;

    public Packet(){
        buffer = new List<byte>();
    }

    public Packet(byte[] arr){
        packet = arr;
    }

    public Packet(PacketTypeServer packet_type_server){
        buffer = new List<byte>();
        writePacketType(packet_type_server);
    }

    public Packet(PacketTypeClient packet_type_client){
        buffer = new List<byte>();
        writePacketType(packet_type_client);
    }


    #region Write into packet
    public void writePacketType(PacketTypeServer packet_type_server){
        buffer.Add((byte)packet_type_server);
    }

    public void writePacketType(PacketTypeClient packet_type_client){
        buffer.Add((byte)packet_type_client);
    }


    public void writeByte(byte b){
        buffer.Add(b);
    }
    public void writeInt(int i){
        buffer.AddRange(BitConverter.GetBytes(i));
    }

    public void writeLong(long l){
        buffer.AddRange(BitConverter.GetBytes(l));
    }

    public void writeFloat(float f){
        buffer.AddRange(BitConverter.GetBytes(f));
    }

    public void writeVector3(Vector3 v){
        writeFloat(v.x);
        writeFloat(v.y);
        writeFloat(v.z);
    }

    public void writeBool(bool b){
        buffer.AddRange(BitConverter.GetBytes(b));
    }

    public void writeString(string s){
        buffer.AddRange(Encoding.ASCII.GetBytes(s));
    }

    public void prependByte(byte b){
        buffer.Insert(0,b);
    }

    public void prependLength(){
        prependByte((byte)buffer.Count);
    }

    public void prependReliability(int msg_id){
        prependByte((byte) msg_id);
        prependByte((byte) PacketTypeClient.RELIABLE);
    }


    

    

    #endregion



    #region read from packet

    public bool isFinishedProcessed(){
        return read_position >= getAbsolutePacketLength();
    }


    public byte readByte(){
        byte b = packet[read_position];
        read_position += 1;
        return b;
    }

    public int readInt(){
        int i = BitConverter.ToInt32(packet,read_position);
        read_position += 4;
        return i;
    }

    public float readFloat(){
        float f = BitConverter.ToSingle(packet,read_position);
        read_position += 4;
        return f;
    }

    public long readLong(){
        long l = BitConverter.ToInt64(packet, read_position);
        read_position += 8;
        return l;
    }


    public Vector3 readVector3(){
        float x = readFloat();
        float y = readFloat();
        float z = readFloat();
        return new Vector3(x,y,z);
    }

    public bool readBool(){
        bool b = BitConverter.ToBoolean(packet,read_position);
        read_position += 1;
        return b;
    }

    // assumes to be the last reading method (no check of string length, no read position increment)
    public string readString(){
        string s = Encoding.ASCII.GetString(packet, read_position, packet.Length - read_position);
        return s;
    }

    #endregion

    public byte[] getWrappedPacket(){
        prependLength();
        return buffer.ToArray();
    }

    public byte[] getWrappedPacketReliable(int msg_id){
        prependReliability(msg_id);
        prependLength();
        return buffer.ToArray();
    }

    public int getAbsolutePacketLength(){
        return packet.Length;
    }

    public byte computeHashStartingFromReadposition(){
        int sum = 0;
        for(int i = read_position; i < packet.Length; i++){
            sum += packet[i];
        }
        return (byte)(sum % 255);
    }

}