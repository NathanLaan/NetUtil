using System;
using System.Net;
using System.IO;

namespace NetUtil.App.Lib.Network
{

    /// <summary>
    /// Based on TCP Packet Header RFC specifications:
    /// 
    /// http://www.ietf.org/rfc/rfc793.txt
    /// </summary>
    class TCPPacketHeader : PacketHeader
    {

        ushort SourcePort { get; set; }
        ushort DestinationPort { get; set; }
        ushort DataOffsetAndFlags { get; set; }
        ushort Window { get; set; }
        ushort UrgentPointer { get; set; }

        uint SequenceNumber { get; set; }
        uint AcknowledgementNumber { get; set; }


        public override void Read(byte[] buffer, int byteCount)
        {
            try
            {
                // Read the buffer via a binary reader on a MemoryStream.
                MemoryStream memoryStream = new MemoryStream(buffer, 0, byteCount);
                BinaryReader binaryReader = new BinaryReader(memoryStream);
           
                SourcePort = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16 ());
                DestinationPort = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16 ());
                SequenceNumber = (uint)IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());
                AcknowledgementNumber = (uint)IPAddress.NetworkToHostOrder(binaryReader.ReadInt32());
                DataOffsetAndFlags = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                Window = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                Checksum = (short)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                UrgentPointer = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());

                // Calculate the header length using the DataOffset value
                HeaderLength = (byte)(DataOffsetAndFlags >> 12);
                HeaderLength *= 4;

                // Message length = total message size - Header length
                MessageLength = (ushort)(byteCount - HeaderLength);

                // Copy the remaining contents of the buffer into the data field, starting at the end of the header.
                Array.Copy(buffer, HeaderLength, data, 0, byteCount - HeaderLength);
            }
            catch
            {
                //
                // TODO: Error handling
                //
            }
        }

    }
}
