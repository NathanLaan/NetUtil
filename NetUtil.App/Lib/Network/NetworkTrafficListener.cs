using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NetUtil.App.Lib.Network
{
    class NetworkTrafficListener
    {

        private Socket mainSocket;

        private byte[] packetData = new byte[4096];
        private bool networkTrafficListenerActive = false;

        public string NetworkInterface { get; set; }
        public bool CaptureIncomingTraffic { get; set; }
        public bool CaptureOutgoingTraffic { get; set; }
        private byte CaptureIncomingTrafficByte
        {
            get
            {
                return (byte)(this.CaptureIncomingTraffic ? 1 : 0);
            }
        }
        private byte CaptureOutgoingTrafficByte
        {
            get
            {
                return (byte)(this.CaptureOutgoingTraffic ? 1 : 0);
            }
        }

        public bool IsListening
        {
            get
            {
                return this.networkTrafficListenerActive;
            }
        }


        //
        // TODO: Add event
        //


        public NetworkTrafficListener()
        {
            //
            // TODO: default
            //
            this.NetworkInterface = "";
            this.CaptureIncomingTraffic = true;
            this.CaptureOutgoingTraffic = true;
        }


        public void Start()
        {
            if (this.networkTrafficListenerActive)
            {
                //
                // TODO: error message?
                //
                return;
            }

            this.networkTrafficListenerActive = true;

            // Network sniffing done via RAW InterNetwork socket using IP protocol
            mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);
            mainSocket.Bind(new IPEndPoint(IPAddress.Parse(this.NetworkInterface), 0));
            mainSocket.SetSocketOption(SocketOptionLevel.IP,
                                       SocketOptionName.HeaderIncluded,
                                       true);

            // Capture ingoing and outgoing packets
            //
            // TODO: set via property
            //
            byte[] optionIncomingValue = new byte[4] { this.CaptureIncomingTrafficByte, 0, 0, 0 };
            byte[] optionOutgoingValue = new byte[4] { this.CaptureOutgoingTrafficByte, 0, 0, 0 };

            mainSocket.IOControl(IOControlCode.ReceiveAll,
                                 optionIncomingValue,
                                 optionOutgoingValue);

            // Start receiving network packets asynchronously
            // Based on MSDN sample code: http://msdn.microsoft.com/en-us/library/bbx2eya8(v=vs.90).aspx
            mainSocket.BeginReceive(packetData,
                0,
                packetData.Length,
                SocketFlags.None,
                new AsyncCallback(this.ReceiveCallback),
                null);
        }

        public void Stop()
        {
            mainSocket.Close();
            mainSocket.Dispose();
        }

        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            try
            {
                int numBytesReceived = mainSocket.EndReceive(asyncResult);

                //
                this.ParsePacketData(this.packetData, numBytesReceived);

                if (this.IsListening)
                {
                    this.packetData = new byte[4096];

                    //Another call to BeginReceive so that we continue to receive the incoming packets
                    mainSocket.BeginReceive(this.packetData,
                        0,
                        this.packetData.Length,
                        SocketFlags.None,
                        new AsyncCallback(ReceiveCallback),
                        null);
                }
            }
            catch
            {
                //
                // TODO: error handling
                //
            }
        }

        private void ParsePacketData(byte[] packetData, int numBytesReceived)
        {



        }



        //
        // TODO: Event code
        //

        public delegate void NetworkTrafficEvent(byte[] data);

        public event NetworkTrafficEvent NetworkTrafficReceived;

        private void OnNetworkTrafficReceived()
        {
            if (this.NetworkTrafficReceived != null)
            {
                this.NetworkTrafficReceived(this.packetData);
            }
        }



    }
}
