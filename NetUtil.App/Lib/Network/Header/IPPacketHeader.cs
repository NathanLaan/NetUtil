using System;
using System.Net;
using System.IO;

namespace NetUtil.App.Lib.Network
{

    /// <summary>
    /// Based on IP Packet Header RFC specifications:
    /// 
    /// http://www.ietf.org/rfc/rfc790.txt
    /// http://www.ietf.org/rfc/rfc791.txt
    /// http://tools.ietf.org/html/rfc2474
    /// http://www.erg.abdn.ac.uk/~gorry/course/inet-pages/dscp.html
    /// http://www.erg.abdn.ac.uk/~gorry/course/inet-pages/ip-packet.html
    /// 
    /// </summary>
    class IPPacketHeader: PacketHeader
    {

        /// <summary>
        /// 4 bits. IP version (currently "4" for IPv4).
        /// </summary>
        int Version { get; set; }

        /// <summary>
        /// 8 bits (1 byte). Differentiated Services Code Point. Formerly the unused "Type of Service (ToS)" indicator.
        /// </summary>
        byte DSCP { get; set; }

        /// <summary>
        /// 16 bit (2 bytes). Packet length = header length + message length.
        /// </summary>
        ushort PacketLength;

        /// <summary>
        /// 3 bits (1 byte) for Flags. 13 bits for Fragment Offset.
        ///     Bit 0: reserved, must be zero
        ///     Bit 1: (DF) 0 = May Fragment,  1 = Don't Fragment.
        ///     Bit 2: (MF) 0 = Last Fragment, 1 = More Fragments.
        /// </summary>
        ushort FlagsAndFragmentOffset;
        ushort Identification;

        /// <summary>
        /// 8 bits (1 byte). Time to Live.
        /// </summary>
        byte TTL;

        /// <summary>
        /// 8 bits. Indicates the higher level protocol used in the data portion of the packet.
        /// 
        /// See: http://www.ietf.org/rfc/rfc790.txt
        /// </summary>
        byte Protocol;

        /// <summary>
        /// 32 bits. Not normally used. If used, HeaderLength will be greater than 5 32-bit words.
        /// </summary>
        uint Options;
        uint SourceAddress;
        uint DestinationAddress;


        public override void Read(byte[] buffer, int byteCount)
        {
            
            try
            {
                // Read the buffer via a binary reader on a MemoryStream.
                MemoryStream memoryStream = new MemoryStream(buffer, 0, byteCount);
                BinaryReader binaryReader = new BinaryReader(memoryStream);

                // Parse out the Version and IHL values
                byte versionAndHeaderLength = binaryReader.ReadByte();
                this.Version = versionAndHeaderLength & 0x0F;
                this.HeaderLength = (byte)(versionAndHeaderLength >> 4);

                this.DSCP = binaryReader.ReadByte();
                this.PacketLength = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                this.Identification = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                this.FlagsAndFragmentOffset = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                this.TTL = binaryReader.ReadByte();
                this.Protocol = binaryReader.ReadByte();
                this.Checksum = IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                this.SourceAddress = (uint)(binaryReader.ReadInt32());
                this.DestinationAddress = (uint)(binaryReader.ReadInt32());

                // Message length = total message size - Header length
                MessageLength = (ushort)(byteCount - HeaderLength);

                //
                // TODO: read Options if needed.
                //
                
                // Copy the remaining contents of the buffer into the data field, starting at the end of the header.
                Array.Copy(buffer, HeaderLength, data, 0, PacketLength - HeaderLength);
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
