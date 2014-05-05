using System;
using System.Collections;
using System.Net;
using System.Runtime.InteropServices;
using Engine;
using UnityEngine;

namespace Engine.Networking {

    public class PingWin : BaseEngineBehavior {
#if !UNITY_FLASH
#if !UNITY_IPHONE

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct ICMP_ECHO_REPLY {
            public uint Address;
            public uint Status;
            public uint RoundTripTime;
            public ushort DataSize;
            public ushort Reserved;
            public IntPtr Data;
            public IP_OPTION_INFORMATION Options;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct IP_OPTION_INFORMATION {
            public byte TTL;
            public byte TOS;
            public byte Flags;
            public byte OptionsSize;
            public IntPtr OptionsData;
            public int RealOptionData;
        }

        private const int IP_SUCCESS = 0;
        private const int IP_BUF_TOO_SMALL = 11001;
        private const int IP_REQ_TIMED_OUT = 11010;

        [DllImport("icmp.dll")]
        private static extern IntPtr IcmpCreateFile();

        [DllImport("icmp.dll")]
        private static extern uint IcmpSendEcho(IntPtr icmpHandle, uint ipAddr, ref int requestData, ushort requestSize, IntPtr optionInfo, ref	ICMP_ECHO_REPLY replyBuffer, uint replySize, int timeout);

        [DllImport("icmp.dll")]
        private static extern bool IcmpCloseHandle(IntPtr icmpHandle);

        public int Ping(string host, int timeout) {
            uint addr = BitConverter.ToUInt32(IPAddress.Parse(host).GetAddressBytes(), 0);
            int req = 123456789;
            ICMP_ECHO_REPLY rep = new ICMP_ECHO_REPLY();
            try {
                IntPtr h = IcmpCreateFile();
                uint retval = IcmpSendEcho(h, addr, ref req, 4, IntPtr.Zero, ref rep, 32, timeout * 1000);
                if (retval == 0) {
                    LogUtil.Log("Error sending ping");
                }
                IcmpCloseHandle(h);
            }
            catch (Exception e) {
                LogUtil.Log("Error doing ping: " + e.ToString());
                return -1;
            }
            return (int)rep.RoundTripTime;
        }

#endif
#endif
    }
}