using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetUtil.App.Lib.Network
{
    abstract class PacketHeader
    {

        /// <summary>
        /// 16 bits. The Checksum can have a negative value.
        /// </summary>
        public short Checksum;

        /// <summary>
        /// 8 bits.
        /// </summary>
        protected byte HeaderLength;

        /// <summary>
        /// Length of the data field.
        /// </summary>
        protected ushort MessageLength;

        protected byte[] data = new byte[4096];

        public abstract void Read(byte[] buffer, int byteCount);

    }
}
