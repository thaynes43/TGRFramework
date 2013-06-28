using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using SlotMachine.Prototype.Tools;

namespace SlotMachine.Prototype.SocketComm
{
    /// <summary>
    /// TCP Server Socket
    /// </summary>
    public class SocketListener : ISocket
    {
        #region Fields
        private object socketLock = new object();
        #endregion

        #region Constructors
        public SocketListener(string logName = null)
        {
            this.ConnectedToClient = false;

            if (!string.IsNullOrEmpty(logName))
            {
                this.Log = new LogTool(logName);
            }
            else
            {
                this.Log = null;
            }

            this.RegisterQueue = new Queue<Tuple<Type, MessageReceivedHandler>>();
            this.ListenForClient();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Client socket server is connected to
        /// </summary>
        public ISocket ClientSocket { get; set; }

        /// <summary>
        /// Socket used to acquire client socket
        /// </summary>
        private Socket ServerSocket { get; set; }

        /// <summary>
        /// Log file name used for all communication through this socket
        /// </summary>
        private ILogTool Log {get; set;}

        /// <summary>
        /// If connection to client established
        /// </summary>
        private bool ConnectedToClient { get; set; }

        /// <summary>
        /// Queue to hold registration info is client is not currently connected
        /// </summary>
        private Queue<Tuple<Type, MessageReceivedHandler>> RegisterQueue { get; set; }
        #endregion

        #region Public Methods
        /// <ineritdoc />
        public void Send(IMessage message)
        {
            lock (this.socketLock)
            {
                this.ClientSocket.Send(message);
            }
        }

        /// <ineritdoc />
        public void RegisterMessage(Type msgType, MessageReceivedHandler handler)
        {
            lock (this.socketLock)
            {
                // Cannot register messages until we are connected.
                if (!this.ConnectedToClient)
                {
                    this.RegisterQueue.Enqueue(new Tuple<Type, MessageReceivedHandler>(msgType, handler));
                }
                else
                {
                    this.ClientSocket.RegisterMessage(msgType, handler);
                }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Attempt to establish connection until client connects
        /// </summary>
        private void ListenForClient()
        {
            lock (this.socketLock)
            {
                this.ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = null;
                ipHostInfo.AddressList.ToList().ForEach((ip) =>
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ipAddress = ip;
                    }
                });

                if (ipAddress == null)
                {
                    throw new ArgumentException("Could not resolve internetwork IP address from {0}", Dns.GetHostName());
                }

                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

                this.ServerSocket.Bind(localEndPoint);
                this.ServerSocket.Listen(100);

                Task.Factory.StartNew(() =>
                {
                    this.ServerSocket.BeginAccept(this.OnConnectToClient, this.ServerSocket);
                });
            }
        }

        #region Callbacks
        /// <summary>
        /// Connection to client established
        /// </summary>
        /// <param name="ar">connection result</param>
        private void OnConnectToClient(IAsyncResult ar)
        {
            lock (this.socketLock)
            {
                this.ConnectedToClient = true;

                Socket listener = (Socket)ar.AsyncState;
                this.ClientSocket = new SocketWrapper(listener.EndAccept(ar), this.Log);

                if (this.Log != null)
                {
                    this.Log.Info("********** Connected to Client **********");
                }

                // Register backlog of messages
                foreach (Tuple<Type, MessageReceivedHandler> tuple in this.RegisterQueue)
                {
                    this.ClientSocket.RegisterMessage(tuple.Item1, tuple.Item2);
                }
            }
        }
        #endregion
        #endregion
    }
}
