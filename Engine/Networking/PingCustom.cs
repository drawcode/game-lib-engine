using System;
using System.Net;
using System.Net.Sockets;
using Engine;
using Engine.Data;
using Engine.Networking;
using Engine.Utility;
using UnityEngine;

namespace Engine.Networking {
    public class PingCustom : BaseEngineBehavior {

//#if !UNITY_FLASH && !UNITY_WEBGL && !UNITY_IPHONE && !UNITY_ANDROID

        private void Start() {
        }

        public int Ping(string host, int timeout) {
            double startTime = 0;
            int pingTime = -1;
            int pingDataSize = 4;               // Ping packet payload, dummy data
            int packetSize = pingDataSize + 8;  // Ping payload + ICMP header

            // Convert IP address string to something we can use
            IPEndPoint ipepServer;
            try {
                ipepServer = new IPEndPoint(IPAddress.Parse(host), 0);
            }
            catch (Exception e) {
                LogUtil.Log("Error parsing IP address: " + e.ToString());
                return -1;
            }
            EndPoint epServer = (ipepServer);

            Socket socket = new Socket(ipepServer.AddressFamily, SocketType.Dgram, ProtocolType.Icmp);
            socket.ReceiveTimeout = timeout;

            // Create packet
            IcmpPacket packet = new IcmpPacket();
            packet.type = 8;        // ICMP echo request
            packet.subCode = 0;
            packet.checkSum = 0;
            packet.identifier = 1;
            packet.sequenceNumber = 0;
            packet.data = new Byte[pingDataSize];
            for (int i = 0; i < pingDataSize; i++) {
                packet.data[i] = (byte)'.';
            }

            Byte[] icmpPacket = new Byte[packetSize];

            // Populate buffer
            Serialize(packet, icmpPacket, pingDataSize);

            // Generate checksum
            UInt16[] checksum = new UInt16[(int)((float)packetSize / 2)];

            //LogUtil.Log("Checksum length is " + checksum.Length);
            int icmpPacketIndex = 0;
            for (int i = 0; i < checksum.Length; i++) {
                checksum[i] = BitConverter.ToUInt16(icmpPacket, icmpPacketIndex);
                icmpPacketIndex += 2;
            }
            packet.checkSum = generateChecksum(checksum, checksum.Length);

            // Repopulate packet with generated checksum
            Byte[] sendBuffer = new Byte[packetSize];
            Serialize(packet, sendBuffer, pingDataSize);

            // Send the ping packet
            startTime = Time.deltaTime;// Network.time;
            //startTime = Network.time;
            if (socket.SendTo(sendBuffer, packetSize, 0, epServer) == 0) {
                LogUtil.Log("Socket Error cannot Send Packet");
            }

            // Receive ping response
            Byte[] rcvBuffer = new Byte[256];
            try {

                //socket.ReceiveFrom(rcvBuffer, ref epServer);
                socket.Receive(rcvBuffer);
                //double stopTime = Network.time;
                double stopTime = Time.deltaTime;// Network.time;
                pingTime = (int)((stopTime - startTime) * 1000);

                //LogUtil.Log("Reply from "+epServer.ToString()+" containing "+rcvBytes+" bytes in "+pingTime+" ms");
            }
            catch (SocketException e) {
                LogUtil.Log("Socket error: " + e.ToString());
            }
            catch (Exception e) {
                LogUtil.Log("Exception occured when receiving " + e.ToString());
            }

            socket.Close();
            return pingTime;

            //return 0;
        }

        // Move contents of packet to a byte array (buffer). pingDataSize defines how large
        // packet.data is.
        private void Serialize(IcmpPacket packet, Byte[] buffer, Int32 pingDataSize) {
            buffer[0] = packet.type;
            buffer[1] = packet.subCode;
            Array.Copy(BitConverter.GetBytes(packet.checkSum), 0, buffer, 2, 2);
            Array.Copy(BitConverter.GetBytes(packet.identifier), 0, buffer, 4, 2);
            Array.Copy(BitConverter.GetBytes(packet.sequenceNumber), 0, buffer, 6, 2);
            Array.Copy(packet.data, 0, buffer, 8, pingDataSize);
        }

        private UInt16 generateChecksum(UInt16[] buffer, int size) {
            Int32 cksum = 0;
            int counter = 0;

            while (size > 0) {
                cksum += Convert.ToInt32(buffer[counter]);
                counter++;
                size--;
            }
            cksum = (cksum >> 16) + (cksum & 0xffff);
            cksum += (cksum >> 16);
            return (UInt16)(~cksum);
        }

        private class IcmpPacket {
            public Byte type;
            public Byte subCode;
            public UInt16 checkSum;
            public UInt16 identifier;
            public UInt16 sequenceNumber;
            public Byte[] data;
        }

//#endif
    }
}