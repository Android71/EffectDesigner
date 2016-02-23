using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using ArtNet.IO;
using ArtNet;

namespace ArtNet.Packets
{
    [Flags]
    public enum PollReplyStatus
    {
        None = 0,
        UBEA = 1,
        RdmCapable = 2,
        ROMBoot = 4
    }

    public class ArtPollReplyPacket:ArtNetPacket
    {
        public ArtPollReplyPacket()
            : base(ArtNetOpCodes.PollReply)
        {
        }

        public ArtPollReplyPacket(ArtNetRecieveData data)
            : base(data)
        {
            
        }

        #region Packet Properties

        private byte[] ipAddress=new byte[4];
        public byte[] IpAddress
        {
            get { return ipAddress; }
            set 
            {
                if (value.Length != 4)
                    throw new ArgumentException("The IP address must be an array of 4 bytes.");

                ipAddress = value; 
            }
        }

        private short port= ArtNetSocket.Port;
        public short Port
        {
            get { return port; }
            set { port = value; }
        }

        private short firmwareVersion = 0x0115;
        public short FirmwareVersion
        {
            get { return firmwareVersion; }
            set { firmwareVersion = value; }
        }

        private byte netSwitch = 0x00;
        public byte NetSwitch
        {
            get { return netSwitch; }
            set { netSwitch = value; }
        }

        private byte subSwitch = 0x00;
        public byte SubSwitch
        {
            get { return subSwitch; }
            set { subSwitch = value; }
        }

        private short oem = 0x0000;
        public short Oem
        {
            get { return oem; }
            set { oem = value; }
        }

        private byte ubeaVersion= 0;
        public byte UbeaVersion
        {
            get { return ubeaVersion; }
            set { ubeaVersion = value; }
        }

        private PollReplyStatus status = PollReplyStatus.ROMBoot;
        public PollReplyStatus Status
        {
            get { return status; }
            set { status = value; }
        }

        private short estaCode = 0x00;//0x414c;
        public short EstaCode
        {
            get { return estaCode; }
            set { estaCode = value; }
        }

        private string shortName = "MTM ArtNode";
        public string ShortName
        {
            get { return shortName; }
            set { shortName = value; }
        }

        private string longName = "DMX Output Port 1";
        public string LongName
        {
            get { return longName; }
            set { longName = value; }
        }

        private string nodeReport= "#0001 [0001] Power On Tests Pass";
        public string NodeReport
        {
            get { return nodeReport; }
            set { nodeReport = value; }
        }

        private short portCount=1;
        public short PortCount
        {
            get { return portCount; }
            set { portCount = value; }
        }

        private byte[] portTypes= new byte[4]{0x80, 0x00, 0x00, 0x00};
        public byte[] PortTypes
        {
            get { return portTypes; }
            set 
            {
                if (value.Length != 4)
                    throw new ArgumentException("The port types must be an array of 4 bytes.");
                portTypes = value; 
            }
        }

        private byte[] goodInput = new byte[4];
        public byte[] GoodInput
        {
            get { return goodInput; }
            set 
            {
                if (value.Length != 4)
                    throw new ArgumentException("The good input must be an array of 4 bytes.");
                goodInput = value; 
            }
        }

        private byte[] goodOutput = new byte[4];

        public byte[] GoodOutput
        {
            get { return goodOutput; }
            set {
                if (value.Length != 4)
                    throw new ArgumentException("The good output must be an array of 4 bytes.");
                goodOutput = value; 
            }
        }

        private byte[] swIn = new byte[4];
        public byte[] SwIn
        {
            get { return swIn; }
            set { swIn = value; }
        }

        private byte[] swOut = new byte[4]{0x00, 0x01, 0x00, 0x00};
        public byte[] SwOut
        {
            get { return swOut; }
            set { swOut = value; }
        }

        private byte swVideo=0;
        public byte SwVideo
        {
            get { return swVideo; }
            set { swVideo = value; }
        }

        private byte swMacro=0;
        public byte SwMacro
        {
            get { return swMacro; }
            set { swMacro = value; }
        }

        private byte swRemote=0;
        public byte SwRemote
        {
            get { return swRemote; }
            set { swRemote = value; }
        }

        private byte style=0;
        public byte Style
        {
            get { return style; }
            set { style = value; }
        }

        private byte[] macAddress = new byte[6] { 0x00, 0x14, 0xD1, 0x16, 0x77, 0x13 };
        public byte[] MacAddress
        {
            get { return macAddress; }
            set 
            {
                if (value.Length != 6) 
                    throw new ArgumentException("The mac address must be an array of 6 bytes.");
                macAddress = value; 
            }
        }

        private byte[] bindIpAddress = new byte[4];
        public byte[] BindIpAddress
        {
            get { return bindIpAddress; }
            set {
                if (value.Length != 4)
                    throw new ArgumentException("The bind IP address must be an array of 4 bytes.");
                bindIpAddress = value; }
        }

        private byte bindIndex= 0;
	    public byte BindIndex
	    {
		    get { return bindIndex;}
		    set { bindIndex = value;}
	    }

        private byte status2 = 0;
        public byte Status2
        {
            get { return status2; }
            set { status2 = value; }
        }
	
	
        #endregion

        public override void ReadData(ArtNetBinaryReader data)
        {
            base.ReadData(data);

            IpAddress = data.ReadBytes(4);
            Port = data.ReadInt16();
            FirmwareVersion = data.ReadNetwork16();
            NetSwitch = data.ReadByte();
            SubSwitch = data.ReadByte();
            Oem = data.ReadNetwork16();
            UbeaVersion = data.ReadByte();
            Status = (PollReplyStatus)data.ReadByte();
            //Status = data.ReadByte();
            EstaCode = data.ReadNetwork16();
            ShortName = data.ReadNetworkString(18);
            LongName = data.ReadNetworkString(64);
            NodeReport = data.ReadNetworkString(64);
            PortCount = data.ReadNetwork16();
            PortTypes = data.ReadBytes(4);
            GoodInput = data.ReadBytes(4);
            GoodOutput = data.ReadBytes(4);
            SwIn = data.ReadBytes(4);
            SwOut = data.ReadBytes(4);
            SwVideo = data.ReadByte();
            SwMacro = data.ReadByte();
            SwRemote = data.ReadByte();
            data.ReadBytes(3);
            Style = data.ReadByte();
            MacAddress = data.ReadBytes(6);
            BindIpAddress = data.ReadBytes(4);
            BindIndex = data.ReadByte();
            Status2 = data.ReadByte();
        }

        public override void WriteData(ArtNetBinaryWriter data)
        {
            base.WriteData(data);
            
            data.Write(IpAddress);
            data.Write(Port);
            data.WriteNetwork(FirmwareVersion);
            data.Write(NetSwitch);
            data.Write(SubSwitch);
            data.WriteNetwork(Oem);
            data.Write(UbeaVersion);
            //byte b = 0x04;
            //data.Write(b);
            data.Write((byte) Status);
            data.Write(EstaCode);
            data.WriteNetwork(ShortName,18);
            data.WriteNetwork(LongName,64);
            data.WriteNetwork(NodeReport,64);
            data.WriteNetwork(PortCount);
            data.Write(PortTypes);
            data.Write(GoodInput);
            data.Write(GoodOutput);
            data.Write(SwIn);
            data.Write(SwOut);
            data.Write(SwVideo);
            data.Write(SwMacro);
            data.Write(SwRemote);
            data.Write(new byte[3]);
            data.Write(Style);
            data.Write(MacAddress);
            data.Write(BindIpAddress);
            data.Write(BindIndex);
            data.Write(Status2);
            data.Write(new byte[26]);
        }
	

    }
}
